namespace exam_Ef_dapper_14_3.Services;

using exam_Ef_dapper_14_3.DTOs;
using exam_Ef_dapper_14_3.Interfaces;
using exam_Ef_dapper_14_3.models;
using Microsoft.Extensions.Logging;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IBookRepository _bookRepo;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepo,
        IBookRepository bookRepo,
        ILogger<OrderService> logger)
    {
        _orderRepo = orderRepo;
        _bookRepo = bookRepo;
        _logger = logger;
    }

    public async Task<Order> PlaceOrderAsync(CreateOrderDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CustomerEmail))
            throw new ArgumentException("Customer email is required.", nameof(dto.CustomerEmail));

        if (dto.Items.Count == 0)
            throw new ArgumentException("An order must contain at least one item.", nameof(dto.Items));

        var orderItems = new List<OrderItem>();

        foreach (var (bookId, quantity) in dto.Items)
        {
            if (quantity <= 0)
                throw new ArgumentException($"Quantity for BookId {bookId} must be greater than zero.");

            var book = await _bookRepo.GetBookByIdAsync(bookId)
                       ?? throw new InvalidOperationException($"Book with ID {bookId} does not exist.");

            if (book.StockQuantity < quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock for '{book.Title}'. " +
                    $"Requested: {quantity}, Available: {book.StockQuantity}.");

            orderItems.Add(new OrderItem { BookId = bookId, Quantity = quantity });

            book.StockQuantity -= quantity;
            await _bookRepo.UpdateBookAsync(book);
        }

        var order = new Order { CustomerEmail = dto.CustomerEmail };

        await _orderRepo.AddOrderAsync(order, orderItems);

        _logger.LogInformation(
            "Order #{OrderId} placed for {Email} with {Count} item(s).",
            order.Id, dto.CustomerEmail, orderItems.Count);

        return order;
    }

    public Task<IEnumerable<Order>> GetAllOrdersAsync() =>
        _orderRepo.GetAllOrdersWithDetailsAsync();

    public async Task CancelOrderAsync(int orderId)
    {
        var orders = await _orderRepo.GetAllOrdersWithDetailsAsync();
        var order = orders.FirstOrDefault(o => o.Id == orderId)
                     ?? throw new InvalidOperationException($"Order #{orderId} not found.");

        foreach (var item in order.OrderItems)
        {
            var book = await _bookRepo.GetBookByIdAsync(item.BookId);
            if (book is not null)
            {
                book.StockQuantity += item.Quantity;
                await _bookRepo.UpdateBookAsync(book);
            }
        }

        await _orderRepo.DeleteOrderAsync(orderId);
        _logger.LogInformation("Order #{OrderId} cancelled. Stock restored.", orderId);
    }

    public Task<IEnumerable<OrderReportDto>> GetOrderReportAsync() =>
        _orderRepo.GetOrderReportDapperAsync();
}