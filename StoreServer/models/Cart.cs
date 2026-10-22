using System;
using System.Collections.Generic;

namespace StoreServer.models;

public partial class Cart
{
    public int IdCart { get; set; }

    public int IdUser { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
