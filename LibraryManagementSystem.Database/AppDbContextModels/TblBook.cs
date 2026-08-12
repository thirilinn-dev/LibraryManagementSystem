using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Database.AppDbContextModels;

public partial class TblBook
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string Genre { get; set; } = null!;

    public string Language { get; set; } = null!;

    public string? Description { get; set; }

    public int TotalBooks { get; set; }

    public int AvailableBooks { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<TblBorrowing> TblBorrowings { get; set; } = new List<TblBorrowing>();
}
