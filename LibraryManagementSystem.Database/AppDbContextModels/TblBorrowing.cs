using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Database.AppDbContextModels;

public partial class TblBorrowing
{
    public int BorrowId { get; set; }

    public int BorrowerId { get; set; }

    public int BookId { get; set; }

    public DateOnly BorrowDate { get; set; }

    public DateOnly DueDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual TblBook Book { get; set; } = null!;

    public virtual TblBorrower Borrower { get; set; } = null!;
}
