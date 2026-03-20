using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using exam_Ef_dapper_14_3.data;
using exam_Ef_dapper_14_3.DTOs;
using exam_Ef_dapper_14_3.Interfaces;
using exam_Ef_dapper_14_3.Repositories;
using exam_Ef_dapper_14_3.Services;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

services.AddDbContext<BookShopDbContext>(options => cd BookstoreManagement.Tests

    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
           .LogTo(Console.WriteLine, LogLevel.Warning));

services.AddScoped<IBookRepository, BookRepository>();
services.AddScoped<IOrderRepository, OrderRepository>();

services.AddScoped<IBookService, BookService>();
services.AddScoped<IOrderService, OrderService>();

var provider = services.BuildServiceProvider();

using var scope = provider.CreateScope();
var bookService = scope.ServiceProvider.GetRequiredService<IBookService>();
var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Scenario 1: Create Book");
var createdBook = await bookService.CreateBookAsync(new CreateBookDto
{
    Title = "Clean Code",
    Price = 29.99m,
    StockQuantity = 10,
    AuthorFullName = "Robert C. Martin",
    AuthorBirthDate = new DateTime(1952, 12, 5)
});
logger.LogInformation("Created: '{Title}' (ID: {Id})", createdBook.Title, createdBook.Id);

logger.LogInformation("\nScenario 2: All Books (EF Core)");
var allBooks = await bookService.GetAllBooksAsync();
foreach (var book in allBooks)
    logger.LogInformation("  [{Id}] {Title} by {Author} — ${Price}",
        book.Id, book.Title, book.Author.FullName, book.Price);

logger.LogInformation("\nScenario 3: Books via Dapper");
var dapperBooks = await bookService.GetBooksDapperAsync();
foreach (var b in dapperBooks)
    logger.LogInformation("  {Title} ({AuthorName}) — ${Price}", b.Title, b.AuthorName, b.Price);

logger.LogInformation("\nScenario 4: Place Order");
try
{
    var order = await orderService.PlaceOrderAsync(new CreateOrderDto
    {
        CustomerEmail = "customer@example.com",
        Items = new Dictionary<int, int> { { createdBook.Id, 2 } }
    });
    logger.LogInformation("Order #{Id} placed successfully.", order.Id);
}
catch (InvalidOperationException ex)
{
    logger.LogWarning("Order failed: {Message}", ex.Message);
}

logger.LogInformation("\nScenario 5: Order Report (Dapper)");
var report = await orderService.GetOrderReportAsync();
foreach (var row in report)
    logger.LogInformation("  Order #{Id} | {Email} | Total: ${Total:F2}",
        row.Id, row.CustomerEmail, row.TotalAmount);