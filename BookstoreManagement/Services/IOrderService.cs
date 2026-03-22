namespace BookstoreManagement.Services;

using BookstoreManagement.DTOs;
using BookstoreManagement.models;

public interface IOrderService
{
    Task<Order> PlaceOrderAsync(CreateOrderDto dto);

    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task CancelOrderAsync(int orderId);
    Task<IEnumerable<OrderReportDto>> GetOrderReportAsync();
}