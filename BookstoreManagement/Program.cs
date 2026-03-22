using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FluentValidation;

using BookstoreManagement.data;
using BookstoreManagement.DTOs;
using BookstoreManagement.Interfaces;
using BookstoreManagement.Repositories;
using BookstoreManagement.Services;
using BookstoreManagement.Validators;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddJsonFile("appsettings.Docker.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

services.AddScoped<IValidator<CreateBookDto>, CreateBookDtoValidator>();
services.AddScoped<IValidator<CreateOrderDto>, CreateOrderDtoValidator>();

services.AddDbContext<BookShopDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null))
    .LogTo(Console.WriteLine, LogLevel.Warning));

services.AddScoped<IBookRepository, BookRepository>();
services.AddScoped<IOrderRepository, OrderRepository>();

services.AddScoped<IBookService, BookService>();
services.AddScoped<IOrderService, OrderService>();

var provider = services.BuildServiceProvider();

using (var migrateScope = provider.CreateScope())
{
    var db = migrateScope.ServiceProvider.GetRequiredService<BookShopDbContext>();
    db.Database.Migrate();
}

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

logger.LogInformation("Scenario 2: All Books (EF Core)");
var allBooks = await bookService.GetAllBooksAsync();
foreach (var book in allBooks)
    logger.LogInformation("  [{Id}] {Title} by {Author} — ${Price}",
        book.Id, book.Title, book.Author.FullName, book.Price);

logger.LogInformation("Scenario 3: Book List (Dapper)");
var dapperBooks = await bookService.GetBooksDapperAsync();
foreach (var b in dapperBooks)
    logger.LogInformation("  {Title} by {AuthorName} — ${Price}",
        b.Title, b.AuthorName, b.Price);

logger.LogInformation("Scenario 4: Place Order");
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
    logger.LogWarning("Order rejected: {Reason}", ex.Message);
}

logger.LogInformation("Scenario 5: Order Report (Dapper)");
var report = await orderService.GetOrderReportAsync();
foreach (var row in report)
    logger.LogInformation("  Order #{Id} | {Email} | Total: ${Total:F2}",
        row.Id, row.CustomerEmail, row.TotalAmount);