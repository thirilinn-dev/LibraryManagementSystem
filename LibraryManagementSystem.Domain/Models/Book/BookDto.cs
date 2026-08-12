namespace LibraryManagementSystem.Domain.Models.Book;

public class BookDto
{
    public int BookId { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public string Language { get; set; } = null!;
    public string? Description { get; set; }
    public int TotalBooks { get; set; }
    public int AvailableBooks { get; set; }
}
