namespace exam_Ef_dapper_14_3.DTOs;

public class OrderReportDto
{
    public int Id { get; init; }
    public string CustomerEmail { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public decimal TotalAmount { get; init; }
}