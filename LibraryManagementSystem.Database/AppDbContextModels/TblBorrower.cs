using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Database.AppDbContextModels;

public partial class TblBorrower
{
    public int BorrowerId { get; set; }

    public string BorrowerName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<TblBorrowing> TblBorrowings { get; set; } = new List<TblBorrowing>();
}
