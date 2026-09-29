using System;
using System.Collections.Generic;

namespace StoreServer.models;

public partial class Category
{
    public int IdCategory { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? CategoryDescription { get; set; }

}
