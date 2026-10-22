using Microsoft.AspNetCore.Mvc;
using StoreServer.models.DTO;
using StoreServer.Services;

namespace StoreServer.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto, CancellationToken cancellationToken = default)
        {
            var user = await _service.LoginAsync(dto, cancellationToken);
            if (user == null)
            {
                return Unauthorized("Неверный логин или пароль");
            }

            return Ok(user);
        }
    }
}
