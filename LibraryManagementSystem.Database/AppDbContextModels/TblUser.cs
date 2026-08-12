using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Database.AppDbContextModels;

public partial class TblUser
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;
}
