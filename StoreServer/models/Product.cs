using System;
using System.Collections.Generic;

namespace StoreServer.models;

public partial class Product
{
    public int IdProduct { get; set; }

    public string ProductName { get; set; } = null!;

    public string? ProductDescription { get; set; }

    public decimal Price { get; set; }

    public int IdCategory { get; set; }

    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<ProductReservation> Reservations { get; set; } = new List<ProductReservation>();
    public int AvailableQuantity { get; set; }
}
