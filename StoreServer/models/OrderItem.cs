using System;
using System.Collections.Generic;

namespace StoreServer.models;

public partial class OrderItem
{
    public int IdOrderItem { get; set; }

    public int IdOrder { get; set; }

    public int IdProduct { get; set; }

    public int Quantity { get; set; }

    public decimal PriceAtMoment { get; set; }

}
