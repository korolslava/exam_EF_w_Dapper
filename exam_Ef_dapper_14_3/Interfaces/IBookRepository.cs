namespace exam_Ef_dapper_14_3.Interfaces;

using exam_Ef_dapper_14_3.DTOs;
using exam_Ef_dapper_14_3.models;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<Book?> GetBookByIdAsync(int id);
    Task AddBookAsync(Book book);
    Task UpdateBookAsync(Book book);
    Task DeleteBookAsync(int id);

    Task<IEnumerable<BookDto>> GetBooksDapperAsync();
}