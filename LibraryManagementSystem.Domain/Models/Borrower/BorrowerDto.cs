namespace LibraryManagementSystem.Domain.Models.Borrower;

public class BorrowerDto
{
    public int BorrowerId { get; set; }
    public string BorrowerName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
}
