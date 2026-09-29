using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StoreServer.models;
// о боже

public partial class WebStoreContext : DbContext
{
    public WebStoreContext()
    {
    }

    public WebStoreContext(DbContextOptions<WebStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=PepegaHS-P1\\SQLEXPRESS;Initial Catalog=web_store;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.IdCart).HasName("PK__Carts__70179490A00CA0D4");

            entity.HasIndex(e => e.IdUser, "UQ__Carts__D7B4671F7E7DC401").IsUnique();

            entity.Property(e => e.IdCart).HasColumnName("ID_cart");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IdUser).HasColumnName("ID_user");

            
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.IdCartItem).HasName("PK__CartItem__2D213196237C5A29");

            entity.HasIndex(e => new { e.IdCart, e.IdProduct }, "UQ__CartItem__5FC06A53C8B85A48").IsUnique();

            entity.Property(e => e.IdCartItem).HasColumnName("ID_cart_item");
            entity.Property(e => e.IdCart).HasColumnName("ID_cart");
            entity.Property(e => e.IdProduct).HasColumnName("ID_product");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");

            
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.IdCategory).HasName("PK__Categori__AC5CA40E3C802D4C");

            entity.HasIndex(e => e.CategoryName, "UQ__Categori__5189E2551C2091C6").IsUnique();

            entity.Property(e => e.IdCategory).HasColumnName("ID_category");
            entity.Property(e => e.CategoryDescription)
                .HasMaxLength(255)
                .HasColumnName("category_description");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .HasColumnName("category_name");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.IdOrder).HasName("PK__Orders__89BFCD597209F84C");

            entity.Property(e => e.IdOrder).HasColumnName("ID_order");
            entity.Property(e => e.IdUser).HasColumnName("ID_user");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(30)
                .HasDefaultValue("Оформлен")
                .HasColumnName("order_status");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("total_amount");

            
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.IdOrderItem).HasName("PK__OrderIte__5B1127FF512A4EA8");

            entity.HasIndex(e => new { e.IdOrder, e.IdProduct }, "UQ__OrderIte__A668339AB53AED9D").IsUnique();

            entity.Property(e => e.IdOrderItem).HasColumnName("ID_order_item");
            entity.Property(e => e.IdOrder).HasColumnName("ID_order");
            entity.Property(e => e.IdProduct).HasColumnName("ID_product");
            entity.Property(e => e.PriceAtMoment)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price_at_moment");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct).HasName("PK__Products__FD7FEC236B472E13");

            entity.HasIndex(e => new { e.ProductName, e.IdCategory }, "UQ__Products__D19FA01F22C622CF").IsUnique();

            entity.Property(e => e.IdProduct).HasColumnName("ID_product");
            entity.Property(e => e.IdCategory).HasColumnName("ID_category");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductDescription)
                .HasMaxLength(500)
                .HasColumnName("product_description");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .HasColumnName("product_name");

            
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("PK__Roles__45DFFBDBC5478621");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__783254B15A38A58C").IsUnique();

            entity.Property(e => e.IdRole).HasColumnName("ID_role");
            entity.Property(e => e.RoleName)
                .HasMaxLength(30)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PK__Users__D7B4671EB4F05EA7");

            entity.HasIndex(e => e.Login, "UQ__Users__7838F2725EC72020").IsUnique();

            entity.Property(e => e.IdUser).HasColumnName("ID_user");
            entity.Property(e => e.IdRole).HasColumnName("ID_role");
            entity.Property(e => e.Login)
                .HasMaxLength(40)
                .HasColumnName("login");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(256)
                .HasColumnName("password_hash");


        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
