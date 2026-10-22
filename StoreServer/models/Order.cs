using System;
using System.Collections.Generic;

namespace StoreServer.models;

public partial class Order
{
    public int IdOrder { get; set; }

    public int IdUser { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string OrderStatus { get; set; } = null!;

    public virtual User User { get; set; } = null!;
    public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
