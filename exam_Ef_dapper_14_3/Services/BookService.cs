namespace exam_Ef_dapper_14_3.Services;

using exam_Ef_dapper_14_3.DTOs;
using exam_Ef_dapper_14_3.Interfaces;
using exam_Ef_dapper_14_3.models;
using Microsoft.Extensions.Logging;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepo;
    private readonly ILogger<BookService> _logger;

    public BookService(IBookRepository bookRepo, ILogger<BookService> logger)
    {
        _bookRepo = bookRepo;
        _logger = logger;
    }

    public async Task<Book> CreateBookAsync(CreateBookDto dto)
    {
        if (dto.Price <= 0)
            throw new ArgumentException("Book price must be greater than zero.", nameof(dto.Price));

        if (dto.StockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative.", nameof(dto.StockQuantity));

        var allBooks = await _bookRepo.GetAllBooksAsync();
        var existingAuthor = allBooks
            .Select(b => b.Author)
            .FirstOrDefault(a => a.FullName == dto.AuthorFullName);

        var author = existingAuthor ?? new Author
        {
            FullName = dto.AuthorFullName,
            BirthDate = dto.AuthorBirthDate
        };

        var book = new Book
        {
            Title = dto.Title,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            Author = author
        };

        await _bookRepo.AddBookAsync(book);
        _logger.LogInformation("Book '{Title}' created with ID {BookId}", book.Title, book.Id);

        return book;
    }

    public Task<IEnumerable<Book>> GetAllBooksAsync() =>
        _bookRepo.GetAllBooksAsync();

    public Task<Book?> GetBookByIdAsync(int id) =>
        _bookRepo.GetBookByIdAsync(id);

    public async Task UpdatePriceAsync(int bookId, decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ArgumentException("Price must be greater than zero.", nameof(newPrice));

        var book = await _bookRepo.GetBookByIdAsync(bookId)
                   ?? throw new InvalidOperationException($"Book with ID {bookId} not found.");

        var oldPrice = book.Price;
        book.Price = newPrice;
        await _bookRepo.UpdateBookAsync(book);

        _logger.LogInformation(
            "Book '{Title}' price updated: {OldPrice} → {NewPrice}",
            book.Title, oldPrice, newPrice);
    }

    public async Task DeleteBookAsync(int bookId)
    {
        var book = await _bookRepo.GetBookByIdAsync(bookId)
                   ?? throw new InvalidOperationException($"Book with ID {bookId} not found.");

        if (book.OrderItems.Count > 0)
            throw new InvalidOperationException(
                $"Cannot delete '{book.Title}' — it is referenced by {book.OrderItems.Count} order(s).");

        await _bookRepo.DeleteBookAsync(bookId);
        _logger.LogInformation("Book '{Title}' (ID {BookId}) deleted.", book.Title, bookId);
    }

    public Task<IEnumerable<BookDto>> GetBooksDapperAsync() =>
        _bookRepo.GetBooksDapperAsync();
}