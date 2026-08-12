using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Database.AppDbContextModels;
using LibraryManagementSystem.Domain.Models.Borrowing;

namespace LibraryManagementSystem.Domain.Features.Borrowing;

public class BorrowingService : IBorrowingService
{
    private readonly AppDbContext _context;

    public BorrowingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BorrowingDto>> GetBorrowingsAsync()
    {
        var borrowings = await _context.TblBorrowings
            .Include(b => b.Book)
            .Include(b => b.Borrower)
            .ToListAsync();

        return borrowings.Select(b => new BorrowingDto
        {
            BorrowId = b.BorrowId,
            BorrowerId = b.BorrowerId,
            BorrowerName = b.Borrower.BorrowerName,
            BookId = b.BookId,
            BookTitle = b.Book.Title,
            BorrowDate = b.BorrowDate,
            DueDate = b.DueDate,
            ReturnDate = b.ReturnDate,
            Status = b.Status
        });
    }

    public async Task<BorrowingDto?> GetBorrowingByIdAsync(int id)
    {
        var borrowing = await _context.TblBorrowings
            .Include(b => b.Book)
            .Include(b => b.Borrower)
            .FirstOrDefaultAsync(b => b.BorrowId == id);

        if (borrowing == null) return null;

        return new BorrowingDto
        {
            BorrowId = borrowing.BorrowId,
            BorrowerId = borrowing.BorrowerId,
            BorrowerName = borrowing.Borrower.BorrowerName,
            BookId = borrowing.BookId,
            BookTitle = borrowing.Book.Title,
            BorrowDate = borrowing.BorrowDate,
            DueDate = borrowing.DueDate,
            ReturnDate = borrowing.ReturnDate,
            Status = borrowing.Status
        };
    }

    public async Task<BorrowingDto> LendBookAsync(CreateBorrowingDto dto)
    {
        var borrower = await _context.TblBorrowers
            .FirstOrDefaultAsync(b => b.BorrowerId == dto.BorrowerId && !b.IsDeleted);
        if (borrower == null)
        {
            throw new KeyNotFoundException("Active borrower not found.");
        }

        var book = await _context.TblBooks
            .FirstOrDefaultAsync(b => b.BookId == dto.BookId && !b.IsDeleted);
        if (book == null)
        {
            throw new KeyNotFoundException("Active book not found.");
        }

        if (book.AvailableBooks <= 0)
        {
            throw new InvalidOperationException("No copies of the book are available for lending.");
        }

        var borrowing = new TblBorrowing
        {
            BorrowerId = dto.BorrowerId,
            BookId = dto.BookId,
            BorrowDate = dto.BorrowDate,
            DueDate = dto.DueDate,
            ReturnDate = null,
            Status = "Borrowed"
        };

        book.AvailableBooks--;

        _context.TblBorrowings.Add(borrowing);
        await _context.SaveChangesAsync();

        borrowing.Book = book;
        borrowing.Borrower = borrower;

        return new BorrowingDto
        {
            BorrowId = borrowing.BorrowId,
            BorrowerId = borrowing.BorrowerId,
            BorrowerName = borrower.BorrowerName,
            BookId = borrowing.BookId,
            BookTitle = book.Title,
            BorrowDate = borrowing.BorrowDate,
            DueDate = borrowing.DueDate,
            ReturnDate = borrowing.ReturnDate,
            Status = borrowing.Status
        };
    }

    public async Task<BorrowingDto?> ReturnBookAsync(int id)
    {
        var borrowing = await _context.TblBorrowings
            .Include(b => b.Book)
            .Include(b => b.Borrower)
            .FirstOrDefaultAsync(b => b.BorrowId == id);

        if (borrowing == null) return null;

        if (borrowing.ReturnDate != null || borrowing.Status == "Returned")
        {
            throw new InvalidOperationException("This borrowing record has already been returned.");
        }

        borrowing.ReturnDate = DateOnly.FromDateTime(DateTime.Today);
        borrowing.Status = "Returned";

        borrowing.Book.AvailableBooks++;

        await _context.SaveChangesAsync();

        return new BorrowingDto
        {
            BorrowId = borrowing.BorrowId,
            BorrowerId = borrowing.BorrowerId,
            BorrowerName = borrowing.Borrower.BorrowerName,
            BookId = borrowing.BookId,
            BookTitle = borrowing.Book.Title,
            BorrowDate = borrowing.BorrowDate,
            DueDate = borrowing.DueDate,
            ReturnDate = borrowing.ReturnDate,
            Status = borrowing.Status
        };
    }

    public async Task<IEnumerable<BorrowingDto>> GetOverdueBorrowingsAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var borrowings = await _context.TblBorrowings
            .Include(b => b.Book)
            .Include(b => b.Borrower)
            .Where(b => b.DueDate < today && b.ReturnDate == null)
            .ToListAsync();

        return borrowings.Select(b => new BorrowingDto
        {
            BorrowId = b.BorrowId,
            BorrowerId = b.BorrowerId,
            BorrowerName = b.Borrower.BorrowerName,
            BookId = b.BookId,
            BookTitle = b.Book.Title,
            BorrowDate = b.BorrowDate,
            DueDate = b.DueDate,
            ReturnDate = b.ReturnDate,
            Status = b.Status
        });
    }
}
