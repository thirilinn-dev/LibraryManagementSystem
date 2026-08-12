using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagementSystem.Domain.Models.Borrowing;

namespace LibraryManagementSystem.Domain.Features.Borrowing;

public interface IBorrowingService
{
    Task<IEnumerable<BorrowingDto>> GetBorrowingsAsync();
    Task<BorrowingDto?> GetBorrowingByIdAsync(int id);
    Task<BorrowingDto> LendBookAsync(CreateBorrowingDto dto);
    Task<BorrowingDto?> ReturnBookAsync(int id);
    Task<IEnumerable<BorrowingDto>> GetOverdueBorrowingsAsync();
}
