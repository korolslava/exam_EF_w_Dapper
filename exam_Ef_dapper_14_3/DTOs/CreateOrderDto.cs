namespace exam_Ef_dapper_14_3.DTOs;

public class CreateOrderDto
{
    public string CustomerEmail { get; init; } = null!;
    public Dictionary<int, int> Items { get; init; } = new();
}
