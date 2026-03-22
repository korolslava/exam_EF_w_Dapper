namespace BookstoreManagement.Services;

using BookstoreManagement.DTOs;
using BookstoreManagement.models;

public interface IBookService
{
    Task<Book> CreateBookAsync(CreateBookDto dto);

    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<Book?> GetBookByIdAsync(int id);

    Task UpdatePriceAsync(int bookId, decimal newPrice);

    Task DeleteBookAsync(int bookId);

    Task<IEnumerable<BookDto>> GetBooksDapperAsync();
}