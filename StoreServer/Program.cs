using StoreServer.Mapping;
using StoreServer.Repositories;
using StoreServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace StoreServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<StoreServer.models.WebStoreContext>(opt =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("con")));

            // DbContext is Scoped: repositories and services share one context
            // per HTTP request, while concurrent requests use different contexts.
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
            builder.Services.AddScoped<ICartItemService, CartItemService>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            builder.Services.AddScoped<IOrderItemService, OrderItemService>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IProductReservationRepository, ProductReservationRepository>();
            builder.Services.AddScoped<IProductReservationService, ProductReservationService>();
            builder.Services.AddAutoMapper(cfg =>
            {
                var licenseKey = builder.Configuration["AutoMapper:LicenseKey"];
                if (!string.IsNullOrWhiteSpace(licenseKey))
                {
                    cfg.LicenseKey = licenseKey;
                }
            }, typeof(MappingProfile));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/", () => Results.Text("StoreServer is running. Open /swagger for the UI.", "text/plain"));

            app.MapControllers();

            app.Run();
        }
    }
}
