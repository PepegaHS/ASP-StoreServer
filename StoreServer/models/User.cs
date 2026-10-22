using System;
using System.Collections.Generic;

namespace StoreServer.models;

public partial class User
{
    public int IdUser { get; set; }

    public string Login { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int IdRole { get; set; }

    public virtual Role Role { get; set; } = null!;
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<ProductReservation> Reservations { get; set; } = new List<ProductReservation>();
}
