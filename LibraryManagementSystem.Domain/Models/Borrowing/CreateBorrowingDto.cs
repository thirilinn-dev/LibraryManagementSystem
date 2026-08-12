using System;

namespace LibraryManagementSystem.Domain.Models.Borrowing;

public class CreateBorrowingDto
{
    public int BorrowerId { get; set; }
    public int BookId { get; set; }
    public DateOnly BorrowDate { get; set; }
    public DateOnly DueDate { get; set; }
}
