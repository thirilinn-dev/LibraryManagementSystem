using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagementSystem.Domain.Models.Borrower;
using LibraryManagementSystem.Domain.Models.Borrowing;

namespace LibraryManagementSystem.Domain.Features.Borrower;

public interface IBorrowerService
{
    Task<IEnumerable<BorrowerDto>> GetBorrowersAsync();
    Task<BorrowerDto?> GetBorrowerByIdAsync(int id);
    Task<BorrowerDto> RegisterBorrowerAsync(CreateBorrowerDto dto);
    Task<BorrowerDto?> UpdateBorrowerAsync(int id, UpdateBorrowerDto dto);
    Task<bool> DeleteBorrowerAsync(int id);
    Task<IEnumerable<BorrowingDto>> GetBorrowingHistoryAsync(int id);
}
