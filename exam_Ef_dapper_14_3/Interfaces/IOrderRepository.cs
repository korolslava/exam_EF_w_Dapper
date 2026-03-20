namespace exam_Ef_dapper_14_3.Interfaces;

using exam_Ef_dapper_14_3.DTOs;
using exam_Ef_dapper_14_3.models;

public interface IOrderRepository
{
    Task AddOrderAsync(Order order, List<OrderItem> items);
    Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync();
    Task DeleteOrderAsync(int id);

    Task<IEnumerable<OrderReportDto>> GetOrderReportDapperAsync();
}