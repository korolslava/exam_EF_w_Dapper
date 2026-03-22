namespace BookstoreManagement.DTOs;

public class OrderReportDto
{
    public int Id { get; init; }
    public string CustomerEmail { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public decimal TotalAmount { get; init; }
}