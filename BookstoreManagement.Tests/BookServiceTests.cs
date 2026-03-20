
using Xunit;
using exam_Ef_dapper_14_3.DTOs;
using exam_Ef_dapper_14_3.Interfaces;
using exam_Ef_dapper_14_3.models;
using exam_Ef_dapper_14_3.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace BookstoreManagement.Tests;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly BookService _sut;

    public BookServiceTests()
    {
        _bookRepoMock = new Mock<IBookRepository>();
        _sut = new BookService(_bookRepoMock.Object, NullLogger<BookService>.Instance);
    }

    [Fact]
    public async Task CreateBookAsync_ValidDto_ReturnsBookWithCorrectTitle()
    {
        var dto = new CreateBookDto
        {
            Title = "Clean Code",
            Price = 29.99m,
            StockQuantity = 10,
            AuthorFullName = "Robert C. Martin",
            AuthorBirthDate = new DateTime(1952, 12, 5)
        };

        _bookRepoMock
            .Setup(r => r.GetAllBooksAsync())
            .ReturnsAsync(new List<Book>());

        _bookRepoMock
            .Setup(r => r.AddBookAsync(It.IsAny<Book>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.CreateBookAsync(dto);

        result.Title.Should().Be("Clean Code");
        result.Price.Should().Be(29.99m);
        result.Author.FullName.Should().Be("Robert C. Martin");
    }

    [Fact]
    public async Task CreateBookAsync_ExistingAuthor_ReusesAuthorInsteadOfCreatingNew()
    {
        var existingAuthor = new Author { Id = 1, FullName = "Robert C. Martin", BirthDate = new DateTime(1952, 12, 5) };
        var existingBook = new Book { Id = 1, Title = "The Clean Coder", Author = existingAuthor };

        _bookRepoMock
            .Setup(r => r.GetAllBooksAsync())
            .ReturnsAsync(new List<Book> { existingBook });

        _bookRepoMock
            .Setup(r => r.AddBookAsync(It.IsAny<Book>()))
            .Returns(Task.CompletedTask);

        var dto = new CreateBookDto
        {
            Title = "Clean Code",
            Price = 19.99m,
            StockQuantity = 5,
            AuthorFullName = "Robert C. Martin",
            AuthorBirthDate = new DateTime(1952, 12, 5)
        };

        var result = await _sut.CreateBookAsync(dto);

        result.Author.Should().BeSameAs(existingAuthor);
    }

    [Fact]
    public async Task CreateBookAsync_NegativePrice_ThrowsArgumentException()
    {
        var dto = new CreateBookDto
        {
            Title = "Bad Book",
            Price = -5m,
            StockQuantity = 10,
            AuthorFullName = "Some Author"
        };

        var act = () => _sut.CreateBookAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*price*");
    }

    [Fact]
    public async Task CreateBookAsync_ZeroPrice_ThrowsArgumentException()
    {
        var dto = new CreateBookDto { Title = "Free Book", Price = 0m, AuthorFullName = "Author" };

        var act = () => _sut.CreateBookAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateBookAsync_NegativeStock_ThrowsArgumentException()
    {
        var dto = new CreateBookDto
        {
            Title = "Book",
            Price = 10m,
            StockQuantity = -1,
            AuthorFullName = "Author"
        };

        var act = () => _sut.CreateBookAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Stock*");
    }

    [Fact]
    public async Task CreateBookAsync_ValidDto_CallsAddBookAsyncExactlyOnce()
    {
        _bookRepoMock.Setup(r => r.GetAllBooksAsync()).ReturnsAsync(new List<Book>());
        _bookRepoMock.Setup(r => r.AddBookAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);

        var dto = new CreateBookDto
        {
            Title = "Test",
            Price = 9.99m,
            StockQuantity = 1,
            AuthorFullName = "Author"
        };

        await _sut.CreateBookAsync(dto);

        _bookRepoMock.Verify(r => r.AddBookAsync(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePriceAsync_ValidPrice_UpdatesBookPrice()
    {
        var book = new Book { Id = 1, Title = "Clean Code", Price = 29.99m, Author = new Author { FullName = "Author" } };

        _bookRepoMock.Setup(r => r.GetBookByIdAsync(1)).ReturnsAsync(book);
        _bookRepoMock.Setup(r => r.UpdateBookAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);

        await _sut.UpdatePriceAsync(1, 39.99m);

        book.Price.Should().Be(39.99m);
    }

    [Fact]
    public async Task UpdatePriceAsync_BookNotFound_ThrowsInvalidOperationException()
    {
        _bookRepoMock.Setup(r => r.GetBookByIdAsync(99)).ReturnsAsync((Book?)null);

        var act = () => _sut.UpdatePriceAsync(99, 19.99m);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*99*");
    }

    [Fact]
    public async Task UpdatePriceAsync_NegativePrice_ThrowsArgumentException()
    {
        var act = () => _sut.UpdatePriceAsync(1, -10m);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task DeleteBookAsync_BookHasOrders_ThrowsInvalidOperationException()
    {
        var book = new Book
        {
            Id = 1,
            Title = "Ordered Book",
            Author = new Author { FullName = "Author" },
            OrderItems = new List<OrderItem> { new() { BookId = 1, OrderId = 1, Quantity = 1 } }
        };

        _bookRepoMock.Setup(r => r.GetBookByIdAsync(1)).ReturnsAsync(book);

        var act = () => _sut.DeleteBookAsync(1);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*order*");
    }

    [Fact]
    public async Task DeleteBookAsync_BookWithNoOrders_CallsDeleteOnRepository()
    {
        var book = new Book
        {
            Id = 1,
            Title = "Free Book",
            Author = new Author { FullName = "Author" },
            OrderItems = new List<OrderItem>()
        };

        _bookRepoMock.Setup(r => r.GetBookByIdAsync(1)).ReturnsAsync(book);
        _bookRepoMock.Setup(r => r.DeleteBookAsync(1)).Returns(Task.CompletedTask);

        await _sut.DeleteBookAsync(1);

        _bookRepoMock.Verify(r => r.DeleteBookAsync(1), Times.Once);
    }
}