$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$serverRoot = Join-Path $repoRoot 'StoreServer'
$temporaryRoot = Join-Path $repoRoot 'tmp'
New-Item -ItemType Directory -Path $temporaryRoot -Force | Out-Null
$previousEnvironment = $env:ASPNETCORE_ENVIRONMENT
$env:ASPNETCORE_ENVIRONMENT = 'Development'
$serverProcess = $null
$client = [System.Net.Http.HttpClient]::new()
$client.Timeout = [TimeSpan]::FromSeconds(5)
try {
    $serverProcess = Start-Process -FilePath 'dotnet' -ArgumentList @(
        'bin/Debug/net10.0/StoreServer.dll', '--urls', 'http://127.0.0.1:5099'
    ) -WorkingDirectory $serverRoot -WindowStyle Hidden -PassThru `
        -RedirectStandardOutput (Join-Path $temporaryRoot 'http-smoke.stdout.log') `
        -RedirectStandardError (Join-Path $temporaryRoot 'http-smoke.stderr.log')
    $ready = $false
    for ($attempt = 0; $attempt -lt 50; $attempt++) {
        try {
            $response = $client.GetAsync('http://127.0.0.1:5099/').GetAwaiter().GetResult()
            if ($response.IsSuccessStatusCode) { $ready = $true; break }
        } catch { }
        Start-Sleep -Milliseconds 200
    }
    if (!$ready) { throw 'Application failed to start.' }
    foreach ($collection in @('Roles', 'Users', 'Categories', 'Products', 'Carts', 'CartItems', 'Orders', 'OrderItems', 'ProductReservations')) {
        $response = $client.GetAsync("http://127.0.0.1:5099/api/${collection}?page=1&pageSize=2").GetAwaiter().GetResult()
        if ([int]$response.StatusCode -ne 200) { throw "$collection returned $($response.StatusCode)" }
        if (!$response.Headers.Contains('X-Total-Count')) { throw "$collection has no total count" }
        $json = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
        if ($json -match 'passwordHash') { throw "$collection exposes a password hash" }
        Write-Output "PASS: GET /api/$collection HTTP 200, X-Total-Count, JSON"
    }
    $swagger = $client.GetStringAsync('http://127.0.0.1:5099/swagger/v1/swagger.json').GetAwaiter().GetResult() | ConvertFrom-Json
    if (!$swagger.paths.'/api/ProductReservations/{id}/cancel') { throw 'Cancel endpoint missing from Swagger.' }
    Write-Output 'PASS: Swagger and production dependency injection'
} finally {
    $client.Dispose()
    if ($serverProcess -and !$serverProcess.HasExited) { Stop-Process -Id $serverProcess.Id -Force }
    $env:ASPNETCORE_ENVIRONMENT = $previousEnvironment
}
