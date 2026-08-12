namespace LibraryManagementSystem.Domain.Models.Borrower;

public class UpdateBorrowerDto
{
    public string BorrowerName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
}
