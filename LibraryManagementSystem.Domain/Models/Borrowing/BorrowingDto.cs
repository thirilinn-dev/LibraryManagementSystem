using System;

namespace LibraryManagementSystem.Domain.Models.Borrowing;

public class BorrowingDto
{
    public int BorrowId { get; set; }
    public int BorrowerId { get; set; }
    public string BorrowerName { get; set; } = null!;
    public int BookId { get; set; }
    public string BookTitle { get; set; } = null!;
    public DateOnly BorrowDate { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly? ReturnDate { get; set; }
    public string Status { get; set; } = null!;
}
