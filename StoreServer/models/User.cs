using System;
using System.Collections.Generic;

namespace StoreServer.models;

public partial class User
{
    public int IdUser { get; set; }

    public string Login { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int IdRole { get; set; }

}
