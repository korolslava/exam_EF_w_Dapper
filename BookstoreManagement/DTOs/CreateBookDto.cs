namespace BookstoreManagement.DTOs;

public class CreateBookDto
{
    public string Title { get; init; } = null!;
    public decimal Price { get; init; }
    public int StockQuantity { get; init; }
    public string AuthorFullName { get; init; } = null!;
    public DateTime AuthorBirthDate { get; init; }
}