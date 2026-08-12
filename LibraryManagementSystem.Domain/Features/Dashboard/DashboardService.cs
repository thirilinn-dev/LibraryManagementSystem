using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Database.AppDbContextModels;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.Domain.Features.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> GetDashboardMetricsAsync()
    {
        var totalBooks = await _context.TblBooks.Where(b => !b.IsDeleted).CountAsync();
        var availableBooks = await _context.TblBooks.Where(b => !b.IsDeleted).SumAsync(b => (int?)b.AvailableBooks) ?? 0;
        var activeBorrowings = await _context.TblBorrowings.Where(b => b.ReturnDate == null).CountAsync();
        
        var today = DateOnly.FromDateTime(DateTime.Today);
        var overdueBooks = await _context.TblBorrowings.Where(b => b.DueDate < today && b.ReturnDate == null).CountAsync();
        
        var registeredBorrowers = await _context.TblBorrowers.Where(b => !b.IsDeleted).CountAsync();

        return new DashboardDto
        {
            TotalBooks = totalBooks,
            AvailableBooks = availableBooks,
            ActiveBorrowings = activeBorrowings,
            OverdueBooks = overdueBooks,
            RegisteredBorrowers = registeredBorrowers
        };
    }
}
