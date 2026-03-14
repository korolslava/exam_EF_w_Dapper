using exam_Ef_dapper_14_3.models;
using exam_Ef_dapper_14_3.data;
using exam_Ef_dapper_14_3.Repositories;
using exam_Ef_dapper_14_3.Interfaces;

class Program
{
    static async Task Main(string[] args)
    {
        using var context = new BookShopDbContext();
        IBookRepository bookrepo = new BookRepository(context);
        IOrderRepository orderepo = new OrderRepository(context);

        Console.WriteLine("\n[EF Core] Add book:");
        var newAuthor = new Author { FullName = "New Author", BirthDate = new DateTime(1980, 1, 1) };
        var newBook = new Book { Title = "New Book", Author = newAuthor, Price = 19.99m, StockQuantity = 56 };

        await bookrepo.AddBookAsync(newBook);

        var books = await bookrepo.GetAllBooksAsync();
        foreach (var book in books)
        {
            Console.WriteLine($"Book: {book.Title}, Author: {book.Author.FullName}, Price: {book.Price}");
        }

        Console.WriteLine("\n[Dapper] Get books:");
        var dapperBooks = bookrepo.GetBooksDapper();
        foreach (var book in dapperBooks)
        {
            Console.WriteLine($"Book: {book.Title}, Author: {book.FullName}, Price: {book.Price}");
        }

        Console.WriteLine("\n[Dapper] Get order report:");
        var orderReport = orderepo.GetOrderReportDapper();
        foreach (var order in orderReport)
        {
            Console.WriteLine($"Order ID: {order.Id}, Customer: {order.CustomerName}, Total: {order.TotalAmount}");
        }
    }
}