using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Database.AppDbContextModels;
using LibraryManagementSystem.Domain.Models.Book;

namespace LibraryManagementSystem.Domain.Features.Book;

public class BookService : IBookService
{
    private readonly AppDbContext _context;

    public BookService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookDto>> GetBooksAsync(string? title = null, string? author = null, string? genre = null)
    {
        var query = _context.TblBooks.Where(b => !b.IsDeleted);

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(b => b.Title.Contains(title));
        }

        if (!string.IsNullOrWhiteSpace(author))
        {
            query = query.Where(b => b.Author.Contains(author));
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            query = query.Where(b => b.Genre.Contains(genre));
        }

        var books = await query.ToListAsync();

        return books.Select(b => new BookDto
        {
            BookId = b.BookId,
            Title = b.Title,
            Author = b.Author,
            Genre = b.Genre,
            Language = b.Language,
            Description = b.Description,
            TotalBooks = b.TotalBooks,
            AvailableBooks = b.AvailableBooks
        });
    }

    public async Task<BookDto?> GetBookByIdAsync(int id)
    {
        var book = await _context.TblBooks
            .FirstOrDefaultAsync(b => b.BookId == id && !b.IsDeleted);

        if (book == null) return null;

        return new BookDto
        {
            BookId = book.BookId,
            Title = book.Title,
            Author = book.Author,
            Genre = book.Genre,
            Language = book.Language,
            Description = book.Description,
            TotalBooks = book.TotalBooks,
            AvailableBooks = book.AvailableBooks
        };
    }

    public async Task<BookDto> AddBookAsync(CreateBookDto dto)
    {
        var book = new TblBook
        {
            Title = dto.Title,
            Author = dto.Author,
            Genre = dto.Genre,
            Language = dto.Language,
            Description = dto.Description,
            TotalBooks = dto.TotalBooks,
            AvailableBooks = dto.TotalBooks,
            IsDeleted = false
        };

        _context.TblBooks.Add(book);
        await _context.SaveChangesAsync();

        return new BookDto
        {
            BookId = book.BookId,
            Title = book.Title,
            Author = book.Author,
            Genre = book.Genre,
            Language = book.Language,
            Description = book.Description,
            TotalBooks = book.TotalBooks,
            AvailableBooks = book.AvailableBooks
        };
    }

    public async Task<BookDto?> UpdateBookAsync(int id, UpdateBookDto dto)
    {
        var book = await _context.TblBooks
            .FirstOrDefaultAsync(b => b.BookId == id && !b.IsDeleted);

        if (book == null) return null;

        int difference = dto.TotalBooks - book.TotalBooks;
        int newAvailable = book.AvailableBooks + difference;
        if (newAvailable < 0)
        {
            throw new InvalidOperationException("Cannot reduce TotalBooks to a value less than the number of currently checked-out books.");
        }

        book.Title = dto.Title;
        book.Author = dto.Author;
        book.Genre = dto.Genre;
        book.Language = dto.Language;
        book.Description = dto.Description;
        book.TotalBooks = dto.TotalBooks;
        book.AvailableBooks = newAvailable;

        await _context.SaveChangesAsync();

        return new BookDto
        {
            BookId = book.BookId,
            Title = book.Title,
            Author = book.Author,
            Genre = book.Genre,
            Language = book.Language,
            Description = book.Description,
            TotalBooks = book.TotalBooks,
            AvailableBooks = book.AvailableBooks
        };
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _context.TblBooks
            .FirstOrDefaultAsync(b => b.BookId == id && !b.IsDeleted);

        if (book == null) return false;

        bool hasActive = await _context.TblBorrowings
            .AnyAsync(b => b.BookId == id && b.ReturnDate == null);

        if (hasActive)
        {
            throw new InvalidOperationException("Cannot delete book with active borrowings.");
        }

        book.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
