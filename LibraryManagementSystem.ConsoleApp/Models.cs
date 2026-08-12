using System;

namespace LibraryManagementSystem.ConsoleApp.Models;

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

public class CreateBookDto
{
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public string Language { get; set; } = null!;
    public string? Description { get; set; }
    public int TotalBooks { get; set; }
}

public class UpdateBookDto
{
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public string Language { get; set; } = null!;
    public string? Description { get; set; }
    public int TotalBooks { get; set; }
}

public class BorrowerDto
{
    public int BorrowerId { get; set; }
    public string BorrowerName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
}

public class CreateBorrowerDto
{
    public string BorrowerName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
}

public class UpdateBorrowerDto
{
    public string BorrowerName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
}

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

public class CreateBorrowingDto
{
    public int BorrowerId { get; set; }
    public int BookId { get; set; }
    public DateOnly BorrowDate { get; set; }
    public DateOnly DueDate { get; set; }
}

public class DashboardDto
{
    public int TotalBooks { get; set; }
    public int AvailableBooks { get; set; }
    public int ActiveBorrowings { get; set; }
    public int OverdueBooks { get; set; }
    public int RegisteredBorrowers { get; set; }
}
