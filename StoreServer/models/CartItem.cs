using System;
using System.Collections.Generic;

namespace StoreServer.models;

public partial class CartItem
{
    public int IdCartItem { get; set; }

    public int IdCart { get; set; }

    public int IdProduct { get; set; }

    public int Quantity { get; set; }

}
