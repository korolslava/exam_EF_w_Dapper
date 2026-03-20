namespace exam_Ef_dapper_14_3.DTOs;

public class CreateBookDto
{
    public string Title { get; init; } = null!;
    public decimal Price { get; init; }
    public int StockQuantity { get; init; }
    public string AuthorFullName { get; init; } = null!;
    public DateTime AuthorBirthDate { get; init; }
}