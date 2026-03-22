namespace BookstoreManagement.Interfaces;

using BookstoreManagement.DTOs;
using BookstoreManagement.models;

public interface IOrderRepository
{
    Task AddOrderAsync(Order order, List<OrderItem> items);
    Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync();
    Task DeleteOrderAsync(int id);

    Task<IEnumerable<OrderReportDto>> GetOrderReportDapperAsync();
}