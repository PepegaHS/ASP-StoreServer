using System;
using System.Collections.Generic;

namespace StoreServer.models;

public partial class Cart
{
    public int IdCart { get; set; }

    public int IdUser { get; set; }

    public DateTime CreatedAt { get; set; }

}
