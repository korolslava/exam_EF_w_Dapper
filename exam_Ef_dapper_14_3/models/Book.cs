namespace exam_Ef_dapper_14_3.models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public DateTime PublishedYear { get; set; }

    public int AuthorId { get; set; }

    public Author Author { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}