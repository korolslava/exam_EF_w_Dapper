namespace BookstoreManagement.DTOs;

public class CreateOrderDto
{
    public string CustomerEmail { get; init; } = null!;
    public Dictionary<int, int> Items { get; init; } = new();
}
