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

   }
