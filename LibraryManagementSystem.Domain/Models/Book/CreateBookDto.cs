namespace LibraryManagementSystem.Domain.Models.Book;

public class CreateBookDto
{
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public string Language { get; set; } = null!;
    public string? Description { get; set; }
    public int TotalBooks { get; set; }
}
