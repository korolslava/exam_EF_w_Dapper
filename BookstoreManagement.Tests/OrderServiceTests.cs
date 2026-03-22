using Xunit;
using BookstoreManagement.DTOs;
using BookstoreManagement.Interfaces;
using BookstoreManagement.models;
using BookstoreManagement.Services;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookstoreManagement.Tests;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock = new();
    private readonly Mock<IBookRepository> _bookRepoMock = new();
    private readonly Mock<IValidator<CreateOrderDto>> _validatorMock = new();
    private readonly Mock<ILogger<OrderService>> _loggerMock = new();
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateOrderDto>(), default))
            .ReturnsAsync(new ValidationResult());

        _sut = new OrderService(
            _orderRepoMock.Object,
            _bookRepoMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task PlaceOrderAsync_ValidOrder_ReturnsOrderWithCorrectEmail()
    {
        var book = new Book
        {
            Id = 1,
            Title = "Clean Code",
            Price = 29.99m,
            StockQuantity = 10,
            Author = new Author { FullName = "Author" }
        };

        _bookRepoMock.Setup(r => r.GetBookByIdAsync(1)).ReturnsAsync(book);
        _bookRepoMock.Setup(r => r.UpdateBookAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);
        _orderRepoMock.Setup(r => r.AddOrderAsync(It.IsAny<Order>(), It.IsAny<List<OrderItem>>()))
                      .Returns(Task.CompletedTask);

        var dto = new CreateOrderDto
        {
            CustomerEmail = "customer@example.com",
            Items = new Dictionary<int, int> { { 1, 2 } }
        };

        var result = await _sut.PlaceOrderAsync(dto);

        result.CustomerEmail.Should().Be("customer@example.com");
    }

    [Fact]
    public async Task PlaceOrderAsync_ValidOrder_DecrementsBookStock()
    {
        var book = new Book
        {
            Id = 1,
            Title = "Clean Code",
            Price = 29.99m,
            StockQuantity = 10,
            Author = new Author { FullName = "Author" }
        };

        _bookRepoMock.Setup(r => r.GetBookByIdAsync(1)).ReturnsAsync(book);
        _bookRepoMock.Setup(r => r.UpdateBookAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);
        _orderRepoMock.Setup(r => r.AddOrderAsync(It.IsAny<Order>(), It.IsAny<List<OrderItem>>()))
                      .Returns(Task.CompletedTask);

        var dto = new CreateOrderDto
        {
            CustomerEmail = "customer@example.com",
            Items = new Dictionary<int, int> { { 1, 3 } }
        };

        await _sut.PlaceOrderAsync(dto);

        book.StockQuantity.Should().Be(7);
    }

    [Fact]
    public async Task PlaceOrderAsync_InsufficientStock_ThrowsInvalidOperationException()
    {
        var book = new Book
        {
            Id = 1,
            Title = "Rare Book",
            Price = 9.99m,
            StockQuantity = 2,
            Author = new Author { FullName = "Author" }
        };

        _bookRepoMock.Setup(r => r.GetBookByIdAsync(1)).ReturnsAsync(book);

        var dto = new CreateOrderDto
        {
            CustomerEmail = "buyer@example.com",
            Items = new Dictionary<int, int> { { 1, 5 } }
        };

        var act = () => _sut.PlaceOrderAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*stock*");
    }

    [Fact]
    public async Task PlaceOrderAsync_BookDoesNotExist_ThrowsInvalidOperationException()
    {
        _bookRepoMock.Setup(r => r.GetBookByIdAsync(999)).ReturnsAsync((Book?)null);

        var dto = new CreateOrderDto
        {
            CustomerEmail = "buyer@example.com",
            Items = new Dictionary<int, int> { { 999, 1 } }
        };

        var act = () => _sut.PlaceOrderAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*999*");
    }

    [Fact]
    public async Task PlaceOrderAsync_EmptyEmail_ThrowsArgumentException()
    {
        var dto = new CreateOrderDto
        {
            CustomerEmail = "",
            Items = new Dictionary<int, int> { { 1, 1 } }
        };

        var act = () => _sut.PlaceOrderAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*email*");
    }

    [Fact]
    public async Task PlaceOrderAsync_EmptyItems_ThrowsArgumentException()
    {
        var dto = new CreateOrderDto
        {
            CustomerEmail = "buyer@example.com",
            Items = new Dictionary<int, int>()
        };

        var act = () => _sut.PlaceOrderAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*item*");
    }

    [Fact]
    public async Task PlaceOrderAsync_ZeroQuantity_ThrowsArgumentException()
    {
        var book = new Book { Id = 1, StockQuantity = 10, Author = new Author { FullName = "Author" } };
        _bookRepoMock.Setup(r => r.GetBookByIdAsync(1)).ReturnsAsync(book);

        var dto = new CreateOrderDto
        {
            CustomerEmail = "buyer@example.com",
            Items = new Dictionary<int, int> { { 1, 0 } }
        };

        var act = () => _sut.PlaceOrderAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Quantity*");
    }

    [Fact]
    public async Task CancelOrderAsync_ExistingOrder_RestoresBookStock()
    {
        var book = new Book
        {
            Id = 1,
            Title = "Clean Code",
            Price = 29.99m,
            StockQuantity = 8,
            Author = new Author { FullName = "Author" }
        };
        var order = new Order
        {
            Id = 1,
            CustomerEmail = "buyer@example.com",
            OrderItems = new List<OrderItem>
            {
                new() { OrderId = 1, BookId = 1, Quantity = 2 }
            }
        };

        _orderRepoMock.Setup(r => r.GetAllOrdersWithDetailsAsync())
                      .ReturnsAsync(new List<Order> { order });
        _bookRepoMock.Setup(r => r.GetBookByIdAsync(1)).ReturnsAsync(book);
        _bookRepoMock.Setup(r => r.UpdateBookAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);
        _orderRepoMock.Setup(r => r.DeleteOrderAsync(1)).Returns(Task.CompletedTask);

        await _sut.CancelOrderAsync(1);

        book.StockQuantity.Should().Be(10);
    }

    [Fact]
    public async Task CancelOrderAsync_OrderNotFound_ThrowsInvalidOperationException()
    {
        _orderRepoMock.Setup(r => r.GetAllOrdersWithDetailsAsync())
                      .ReturnsAsync(new List<Order>());

        var act = () => _sut.CancelOrderAsync(42);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*42*");
    }

    [Fact]
    public async Task CancelOrderAsync_ExistingOrder_CallsDeleteOnRepository()
    {
        var order = new Order
        {
            Id = 5,
            CustomerEmail = "test@example.com",
            OrderItems = new List<OrderItem>()
        };

        _orderRepoMock.Setup(r => r.GetAllOrdersWithDetailsAsync())
                      .ReturnsAsync(new List<Order> { order });
        _orderRepoMock.Setup(r => r.DeleteOrderAsync(5)).Returns(Task.CompletedTask);

        await _sut.CancelOrderAsync(5);

        _orderRepoMock.Verify(r => r.DeleteOrderAsync(5), Times.Once);
    }
}