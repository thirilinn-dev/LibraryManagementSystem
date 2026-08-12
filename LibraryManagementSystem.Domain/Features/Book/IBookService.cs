using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagementSystem.Domain.Models.Book;

namespace LibraryManagementSystem.Domain.Features.Book;

public interface IBookService
{
    Task<IEnumerable<BookDto>> GetBooksAsync(string? title = null, string? author = null, string? genre = null);
    Task<BookDto?> GetBookByIdAsync(int id);
    Task<BookDto> AddBookAsync(CreateBookDto dto);
    Task<BookDto?> UpdateBookAsync(int id, UpdateBookDto dto);
    Task<bool> DeleteBookAsync(int id);
}
