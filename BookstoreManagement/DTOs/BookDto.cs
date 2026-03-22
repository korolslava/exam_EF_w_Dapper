namespace BookstoreManagement.DTOs;

public class BookDto
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public decimal Price { get; init; }
    public string AuthorName { get; init; } = null!;
}