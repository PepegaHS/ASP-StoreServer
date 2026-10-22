using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StoreServer.Controllers;
using StoreServer.Mapping;
using StoreServer.models;
using StoreServer.models.DTO;
using StoreServer.Repositories;
using StoreServer.Services;

// Integration checks use only the application's SQL Server provider.
// The surrounding test transaction is always rolled back. Business methods
// themselves do not open transactions and call SaveChangesAsync exactly once.
var configuration = new ConfigurationBuilder()
    .AddJsonFile(Path.GetFullPath("StoreServer/appsettings.json"))
    .AddEnvironmentVariables().Build();
var options = new DbContextOptionsBuilder<WebStoreContext>()
    .UseSqlServer(configuration.GetConnectionString("con")).Options;
await using var context = new CountingContext(options);
await using var transaction = await context.Database.BeginTransactionAsync();
var registrations = new ServiceCollection();
registrations.AddLogging();
registrations.AddSingleton<WebStoreContext>(context);
registrations.AddAutoMapper(cfg => { }, typeof(MappingProfile));
foreach (var type in typeof(ProductService).Assembly.GetTypes()
    .Where(t => t.IsClass && !t.IsAbstract &&
        (t.Namespace == "StoreServer.Services" || t.Namespace == "StoreServer.Repositories")))
{
    foreach (var contract in type.GetInterfaces()) registrations.AddScoped(contract, type);
}
using var provider = registrations.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true });
using var scope = provider.CreateScope();
var services = scope.ServiceProvider;
var mapper = services.GetRequiredService<IMapper>();
mapper.ConfigurationProvider.AssertConfigurationIsValid();
Console.WriteLine("PASS: all AutoMapper profiles and service dependencies");

var ct = CancellationToken.None;
var suffix = Guid.NewGuid().ToString("N")[..10];
var role = new Role { RoleName = "test_" + suffix };
var category = new Category { CategoryName = "test_" + suffix };
context.AddRange(role, category);
await context.SaveChangesAsync();
var user = new User { Login = "test_" + suffix, PasswordHash = BCrypt.Net.BCrypt.HashPassword("test_password"), IdRole = role.IdRole };
var product = new Product { ProductName = "test_" + suffix, IdCategory = category.IdCategory, Price = 100, AvailableQuantity = 20 };
context.AddRange(user, product);
await context.SaveChangesAsync();
var cart = new Cart { IdUser = user.IdUser, CreatedAt = DateTime.UtcNow };
var order = new Order { IdUser = user.IdUser, OrderDate = DateTime.UtcNow, OrderStatus = "test", TotalAmount = 100 };
context.AddRange(cart, order);
await context.SaveChangesAsync();

try
{
    await Crud<RoleDTO, CreateRoleDTO, UpdateRoleDTO, PatchRoleDTO>(
        services.GetRequiredService<IRoleService>(),
        new() { RoleName = "crud_" + suffix }, new() { RoleName = "put_" + suffix }, new() { RoleName = "patch_" + suffix }, x => x.IdRole);
    await Crud<CategoryDTO, CreateCategoryDTO, UpdateCategoryDTO, PatchCategoryDTO>(
        services.GetRequiredService<ICategoryService>(),
        new() { CategoryName = "crud_" + suffix }, new() { CategoryName = "put_" + suffix }, new() { CategoryName = "patch_" + suffix }, x => x.IdCategory);
    await Crud<UserDTO, CreateUserDTO, UpdateUserDTO, PatchUserDTO>(
        services.GetRequiredService<IUserService>(),
        new() { Login = "crud_" + suffix, Password = "create", IdRole = role.IdRole },
        new() { Login = "put_" + suffix, Password = "update", IdRole = role.IdRole },
        new() { Password = "patch" }, x => x.IdUser);
    await Crud<ProductDTO, CreateProductDTO, UpdateProductDTO, PatchProductDTO>(
        services.GetRequiredService<IProductService>(),
        new() { ProductName = "crud_" + suffix, IdCategory = category.IdCategory, Price = 10, AvailableQuantity = 5 },
        new() { ProductName = "put_" + suffix, IdCategory = category.IdCategory, Price = 20, AvailableQuantity = 6 },
        new() { Price = 30 }, x => x.IdProduct);

    // A separate user avoids the existing one-cart-per-user unique constraint.
    var cartUser = new User { Login = "cart_" + suffix, PasswordHash = user.PasswordHash, IdRole = role.IdRole };
    context.Add(cartUser);
    await context.SaveChangesAsync();
    await Crud<CartDTO, CreateCartDTO, UpdateCartDTO, PatchCartDTO>(
        services.GetRequiredService<ICartService>(),
        new() { IdUser = cartUser.IdUser, CreatedAt = DateTime.UtcNow },
        new() { IdUser = cartUser.IdUser, CreatedAt = DateTime.UtcNow },
        new() { CreatedAt = DateTime.UtcNow }, x => x.IdCart);
    await Crud<CartItemDTO, CreateCartItemDTO, UpdateCartItemDTO, PatchCartItemDTO>(
        services.GetRequiredService<ICartItemService>(),
        new() { IdCart = cart.IdCart, IdProduct = product.IdProduct, Quantity = 1 },
        new() { IdCart = cart.IdCart, IdProduct = product.IdProduct, Quantity = 2 },
        new() { Quantity = 3 }, x => x.IdCartItem);
    await Crud<OrderDTO, CreateOrderDTO, UpdateOrderDTO, PatchOrderDTO>(
        services.GetRequiredService<IOrderService>(),
        new() { IdUser = user.IdUser, OrderDate = DateTime.UtcNow, OrderStatus = "created", TotalAmount = 10 },
        new() { IdUser = user.IdUser, OrderDate = DateTime.UtcNow, OrderStatus = "updated", TotalAmount = 20 },
        new() { OrderStatus = "patched" }, x => x.IdOrder);
    await Crud<OrderItemDTO, CreateOrderItemDTO, UpdateOrderItemDTO, PatchOrderItemDTO>(
        services.GetRequiredService<IOrderItemService>(),
        new() { IdOrder = order.IdOrder, IdProduct = product.IdProduct, Quantity = 1, PriceAtMoment = 10 },
        new() { IdOrder = order.IdOrder, IdProduct = product.IdProduct, Quantity = 2, PriceAtMoment = 20 },
        new() { Quantity = 3 }, x => x.IdOrderItem);

    context.ChangeTracker.Clear();
    var productRead = await services.GetRequiredService<IProductService>().GetByIdAsync(product.IdProduct, ct);
    Check(productRead?.Category.IdCategory == category.IdCategory, "nested product/category DTO");
    var roleRead = await services.GetRequiredService<IRoleService>().GetByIdAsync(role.IdRole, ct);
    Check(roleRead!.Users.Any(x => x.IdUser == user.IdUser), "nested role/users DTO");
    var auth = services.GetRequiredService<IAuthService>();
    Check(await auth.LoginAsync(new() { Login = user.Login, Password = "test_password" }, ct) != null, "login success");
    Check(await auth.LoginAsync(new() { Login = user.Login, Password = "wrong" }, ct) == null, "login failure");

    var reservations = services.GetRequiredService<IProductReservationService>();
    context.SaveCalls = 0;
    var reserved = await reservations.CreateAsync(new() { IdProduct = product.IdProduct, IdUser = user.IdUser, Quantity = 4 }, ct);
    Check(context.SaveCalls == 1 && reserved.Product.AvailableQuantity == 16, "reserve: two entities, one save");
    Check(reserved.User.IdUser == user.IdUser && reserved.Product.Category.IdCategory == category.IdCategory, "reservation nested data");
    context.SaveCalls = 0;
    await Expect<InvalidOperationException>(() => reservations.CreateAsync(new() { IdProduct = product.IdProduct, IdUser = user.IdUser, Quantity = 100 }, ct));
    Check(context.SaveCalls == 0, "insufficient stock never saves");
    await Expect<InvalidOperationException>(() => reservations.CreateAsync(new() { IdProduct = product.IdProduct, IdUser = user.IdUser, Quantity = 0 }, ct));
    await Expect<InvalidOperationException>(() => reservations.CreateAsync(new() { IdProduct = product.IdProduct, IdUser = int.MaxValue, Quantity = 1 }, ct));
    await reservations.UpdateAsync(reserved.IdProductReservation, new() { Quantity = 5 }, ct);
    var changed = await reservations.PatchAsync(reserved.IdProductReservation, new() { Quantity = 2 }, ct);
    Check(changed?.Product.AvailableQuantity == 18, "reservation PUT/PATCH adjust stock");
    var reservationController = new ProductReservationsController(reservations)
    { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    Check(await reservationController.Create(new() { IdProduct = product.IdProduct, IdUser = user.IdUser, Quantity = 100 }) is BadRequestObjectResult, "insufficient stock HTTP 400");
    context.SaveCalls = 0;
    Check(await reservationController.Cancel(reserved.IdProductReservation) is NoContentResult, "cancel HTTP 204");
    Check(context.SaveCalls == 1, "cancel: two entities, one save");
    context.ChangeTracker.Clear();
    Check((await context.Products.FindAsync(product.IdProduct))!.AvailableQuantity == 20, "cancel restores stock");
    Check((await reservations.GetByIdAsync(reserved.IdProductReservation, ct))!.CancelledAt != null, "cancel closes event");
    context.SaveCalls = 0;
    Check(await reservationController.Cancel(reserved.IdProductReservation) is NotFoundResult, "repeat cancel HTTP 404");
    Check(await reservationController.Cancel(int.MaxValue) is NotFoundResult, "missing cancel HTTP 404");
    Check(context.SaveCalls == 0, "no second return of stock");
    Check(await reservations.UpdateAsync(reserved.IdProductReservation, new() { Quantity = 1 }, ct) == null, "closed event cannot be edited");

    // Force the second write to fail: SQL Server must roll back the stock write too.
    var repository = services.GetRequiredService<IProductReservationRepository>();
    var stock = await repository.FindProductAsync(product.IdProduct, ct);
    stock!.AvailableQuantity--;
    await repository.AddAsync(new() { IdProduct = product.IdProduct, IdUser = int.MaxValue, Quantity = 1, CreatedAt = DateTime.UtcNow }, ct);
    await Expect<InvalidOperationException>(() => repository.SaveChangesAsync(ct));
    context.ChangeTracker.Clear();
    Check((await context.Products.FindAsync(product.IdProduct))!.AvailableQuantity == 20, "failed event insert rolls back stock write");

    // Emulate a concurrent stock update after the product was loaded.
    var active = await reservations.CreateAsync(new() { IdProduct = product.IdProduct, IdUser = user.IdUser, Quantity = 1 }, ct);
    await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Products SET available_quantity = available_quantity - 1 WHERE ID_product = {product.IdProduct}");
    await Expect<InvalidOperationException>(() => reservations.CancelAsync(active.IdProductReservation, ct));
    context.ChangeTracker.Clear();
    Check((await reservations.GetByIdAsync(active.IdProductReservation, ct))!.CancelledAt == null, "concurrent conflict does not close event");
    Check((await context.Products.FindAsync(product.IdProduct))!.AvailableQuantity == 18, "concurrent stock value preserved");

    var controller = new ProductsController(services.GetRequiredService<IProductService>())
    { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    Check(await controller.GetAll() is OkObjectResult && int.Parse(controller.Response.Headers["X-Total-Count"]!) > 0, "list HTTP 200 and X-Total-Count");
    Check(await controller.GetAll(page: 0) is BadRequestObjectResult, "invalid pagination HTTP 400");
    Check(await controller.GetById(int.MaxValue) is NotFoundResult, "missing item HTTP 404");
    using var cancelled = new CancellationTokenSource();
    cancelled.Cancel();
    await Expect<OperationCanceledException>(() => services.GetRequiredService<IProductService>().GetAllAsync(1, 10, cancelled.Token));
    Console.WriteLine("PASS: business operations, atomicity, concurrency, HTTP statuses, cancellation");
}
finally
{
    await transaction.RollbackAsync();
    Console.WriteLine("Test data rolled back in MSSQL.");
}

async Task Crud<TRead, TCreate, TUpdate, TPatch>(dynamic service, TCreate create, TUpdate update, TPatch patch, Func<TRead, int> getId)
{
    TRead created = await service.CreateAsync(create!, ct);
    var id = getId(created);
    Check(id > 0, typeof(TRead).Name + " create identity");
    context.ChangeTracker.Clear();
    TRead read = await service.GetByIdAsync(id, ct);
    Check(read != null, typeof(TRead).Name + " get");
    CheckFields(create!, read!);
    (List<TRead> Items, int Total) page = await service.GetAllAsync(1, 2, ct);
    Check(page.Items.Count <= 2 && page.Total >= 1, typeof(TRead).Name + " pagination/count");
    Check(await service.UpdateAsync(id, update!, ct) != null, typeof(TRead).Name + " PUT");
    context.ChangeTracker.Clear();
    TRead updated = await service.GetByIdAsync(id, ct);
    CheckFields(update!, updated!);
    Check(await service.PatchAsync(id, patch!, ct) != null, typeof(TRead).Name + " PATCH");
    context.ChangeTracker.Clear();
    TRead patched = await service.GetByIdAsync(id, ct);
    CheckFields(patch!, patched!);
    // Omitted PATCH fields keep the values saved by PUT.
    foreach (var property in typeof(TRead).GetProperties().Where(p =>
        p.PropertyType.IsValueType || p.PropertyType == typeof(string)))
    {
        if (patch!.GetType().GetProperty(property.Name)?.GetValue(patch) != null) continue;
        Check(Equals(property.GetValue(updated), property.GetValue(patched)), property.Name + " PATCH preservation");
    }
    Check(await service.DeleteAsync(id, ct), typeof(TRead).Name + " delete");
    Check(await service.GetByIdAsync(id, ct) == null, typeof(TRead).Name + " missing after delete");
    Check(!await service.DeleteAsync(id, ct), typeof(TRead).Name + " repeated delete");
    Console.WriteLine("PASS: " + typeof(TRead).Name + " CRUD");
}

static void CheckFields(object input, object output)
{
    foreach (var property in input.GetType().GetProperties())
    {
        var expected = property.GetValue(input);
        var target = output.GetType().GetProperty(property.Name);
        if (expected == null || target == null || expected is DateTime) continue;
        Check(Equals(expected, target.GetValue(output)), output.GetType().Name + "." + property.Name);
    }
}

static void Check(bool condition, string name)
{
    if (!condition) throw new Exception("FAIL: " + name);
}

static async Task Expect<TException>(Func<Task> operation) where TException : Exception
{
    try { await operation(); }
    catch (TException) { return; }
    throw new Exception("Expected " + typeof(TException).Name);
}

sealed class CountingContext(DbContextOptions<WebStoreContext> options) : WebStoreContext(options)
{
    public int SaveCalls { get; set; }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveCalls++;
        return base.SaveChangesAsync(cancellationToken);
    }
}
