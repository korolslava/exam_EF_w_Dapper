namespace BookstoreManagement.Interfaces;

using BookstoreManagement.DTOs;
using BookstoreManagement.models;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<Book?> GetBookByIdAsync(int id);
    Task AddBookAsync(Book book);
    Task UpdateBookAsync(Book book);
    Task DeleteBookAsync(int id);

    Task<IEnumerable<BookDto>> GetBooksDapperAsync();
}