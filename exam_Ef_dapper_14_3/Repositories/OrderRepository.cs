namespace exam_Ef_dapper_14_3.Repositories;

using exam_Ef_dapper_14_3.data;
using exam_Ef_dapper_14_3.DTOs;
using exam_Ef_dapper_14_3.Interfaces;
using exam_Ef_dapper_14_3.models;
using Microsoft.EntityFrameworkCore;
using Dapper;

public class OrderRepository : IOrderRepository
{
    private readonly BookShopDbContext _context;

    public OrderRepository(BookShopDbContext context) => _context = context;

    public async Task AddOrderAsync(Order order, List<OrderItem> items)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in items)
                {
                    item.OrderId = order.Id;
                    _context.OrderItems.Add(item);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    public async Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync() =>
        await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                    .ThenInclude(b => b.Author)
            .ToListAsync();

    public async Task DeleteOrderAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order is not null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<OrderReportDto>> GetOrderReportDapperAsync()
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await _context.Database.OpenConnectionAsync();

        const string sql = """
            SELECT   o.Id,
                     o.CustomerEmail,
                     o.CreatedAt,
                     SUM(oi.Quantity * b.Price) AS TotalAmount
            FROM     Orders o
            JOIN     OrderItems oi ON o.Id  = oi.OrderId
            JOIN     Books b       ON oi.BookId = b.Id
            GROUP BY o.Id, o.CustomerEmail, o.CreatedAt
            ORDER BY o.CreatedAt DESC
            """;
        return await connection.QueryAsync<OrderReportDto>(sql);
    }
}