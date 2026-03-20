namespace exam_Ef_dapper_14_3.Services;

using exam_Ef_dapper_14_3.DTOs;
using exam_Ef_dapper_14_3.models;

public interface IBookService
{
    Task<Book> CreateBookAsync(CreateBookDto dto);

    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<Book?> GetBookByIdAsync(int id);

    Task UpdatePriceAsync(int bookId, decimal newPrice);

    Task DeleteBookAsync(int bookId);

    Task<IEnumerable<BookDto>> GetBooksDapperAsync();
}