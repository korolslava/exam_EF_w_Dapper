namespace exam_Ef_dapper_14_3.Services;

using exam_Ef_dapper_14_3.DTOs;
using exam_Ef_dapper_14_3.models;

public interface IOrderService
{
    Task<Order> PlaceOrderAsync(CreateOrderDto dto);

    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task CancelOrderAsync(int orderId);
    Task<IEnumerable<OrderReportDto>> GetOrderReportAsync();
}