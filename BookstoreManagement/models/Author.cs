namespace BookstoreManagement.models;

public class Author
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public DateTime BirthDate { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}