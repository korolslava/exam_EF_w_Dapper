namespace exam_Ef_dapper_14_3.Repositories;

using exam_Ef_dapper_14_3.data;
using exam_Ef_dapper_14_3.DTOs;
using exam_Ef_dapper_14_3.Interfaces;
using exam_Ef_dapper_14_3.models;
using Microsoft.EntityFrameworkCore;
using Dapper;

public class BookRepository : IBookRepository
{
    private readonly BookShopDbContext _context;

    public BookRepository(BookShopDbContext context) => _context = context;

    public async Task<IEnumerable<Book>> GetAllBooksAsync() =>
        await _context.Books.Include(b => b.Author).ThenInclude(a => a.Books).ToListAsync();

    public async Task<Book?> GetBookByIdAsync(int id) =>
        await _context.Books
            .Include(b => b.Author)
            .Include(b => b.OrderItems)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task AddBookAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBookAsync(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is not null)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<BookDto>> GetBooksDapperAsync()
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await _context.Database.OpenConnectionAsync();

        const string sql = """
            SELECT b.Id, b.Title, b.Price, a.FullName AS AuthorName
            FROM   Books b
            JOIN   Authors a ON b.AuthorId = a.Id
            ORDER BY b.Title
            """;
        return await connection.QueryAsync<BookDto>(sql);
    }
}