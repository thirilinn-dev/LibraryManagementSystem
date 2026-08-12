using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Database.AppDbContextModels;
using LibraryManagementSystem.Domain.Models.Borrower;
using LibraryManagementSystem.Domain.Models.Borrowing;

namespace LibraryManagementSystem.Domain.Features.Borrower;

public class BorrowerService : IBorrowerService
{
    private readonly AppDbContext _context;

    public BorrowerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BorrowerDto>> GetBorrowersAsync()
    {
        var borrowers = await _context.TblBorrowers
            .Where(b => !b.IsDeleted)
            .ToListAsync();

        return borrowers.Select(b => new BorrowerDto
        {
            BorrowerId = b.BorrowerId,
            BorrowerName = b.BorrowerName,
            Phone = b.Phone,
            Email = b.Email
        });
    }

    public async Task<BorrowerDto?> GetBorrowerByIdAsync(int id)
    {
        var borrower = await _context.TblBorrowers
            .FirstOrDefaultAsync(b => b.BorrowerId == id && !b.IsDeleted);

        if (borrower == null) return null;

        return new BorrowerDto
        {
            BorrowerId = borrower.BorrowerId,
            BorrowerName = borrower.BorrowerName,
            Phone = borrower.Phone,
            Email = borrower.Email
        };
    }

    public async Task<BorrowerDto> RegisterBorrowerAsync(CreateBorrowerDto dto)
    {
        var borrower = new TblBorrower
        {
            BorrowerName = dto.BorrowerName,
            Phone = dto.Phone,
            Email = dto.Email,
            IsDeleted = false
        };

        _context.TblBorrowers.Add(borrower);
        await _context.SaveChangesAsync();

        return new BorrowerDto
        {
            BorrowerId = borrower.BorrowerId,
            BorrowerName = borrower.BorrowerName,
            Phone = borrower.Phone,
            Email = borrower.Email
        };
    }

    public async Task<BorrowerDto?> UpdateBorrowerAsync(int id, UpdateBorrowerDto dto)
    {
        var borrower = await _context.TblBorrowers
            .FirstOrDefaultAsync(b => b.BorrowerId == id && !b.IsDeleted);

        if (borrower == null) return null;

        borrower.BorrowerName = dto.BorrowerName;
        borrower.Phone = dto.Phone;
        borrower.Email = dto.Email;

        await _context.SaveChangesAsync();

        return new BorrowerDto
        {
            BorrowerId = borrower.BorrowerId,
            BorrowerName = borrower.BorrowerName,
            Phone = borrower.Phone,
            Email = borrower.Email
        };
    }

    public async Task<bool> DeleteBorrowerAsync(int id)
    {
        var borrower = await _context.TblBorrowers
            .FirstOrDefaultAsync(b => b.BorrowerId == id && !b.IsDeleted);

        if (borrower == null) return false;

        // Check if borrower has active borrowings
        bool hasActive = await _context.TblBorrowings
            .AnyAsync(b => b.BorrowerId == id && b.ReturnDate == null);

        if (hasActive)
        {
            throw new InvalidOperationException("Cannot delete borrower with active borrowings.");
        }

        borrower.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<BorrowingDto>> GetBorrowingHistoryAsync(int id)
    {
        var borrowerExists = await _context.TblBorrowers.AnyAsync(b => b.BorrowerId == id && !b.IsDeleted);
        if (!borrowerExists)
        {
            return Enumerable.Empty<BorrowingDto>();
        }

        var borrowings = await _context.TblBorrowings
            .Include(b => b.Book)
            .Include(b => b.Borrower)
            .Where(b => b.BorrowerId == id)
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
