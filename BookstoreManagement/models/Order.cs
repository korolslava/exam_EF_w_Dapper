namespace BookstoreManagement.models;

public class Order
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
