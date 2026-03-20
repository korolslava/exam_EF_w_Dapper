namespace exam_Ef_dapper_14_3.data;

using exam_Ef_dapper_14_3.models;
using Microsoft.EntityFrameworkCore;

public class BookShopDbContext : DbContext
{
    public BookShopDbContext(DbContextOptions<BookShopDbContext> options) : base(options) { }

    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(author =>
        {
            author.Property(e => e.FullName).HasMaxLength(100).IsRequired();
            author.HasIndex(e => e.FullName).IsUnique();
        });

        modelBuilder.Entity<Book>(book =>
        {
            book.Property(e => e.Title).HasMaxLength(200).IsRequired();
            book.HasIndex(e => e.Title);
            book.Property(e => e.Price).HasColumnType("decimal(10,2)");

            book.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Book_Price", "Price > 0");
                t.HasCheckConstraint("CK_Book_StockQuantity", "StockQuantity >= 0");
            });

            book.HasOne(e => e.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Order>(order =>
        {
            order.Property(e => e.CustomerEmail).IsRequired();
            order.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<OrderItem>(item =>
        {
            item.HasKey(e => new { e.OrderId, e.BookId });
            item.ToTable(t => t.HasCheckConstraint("CK_OrderItem_Quantity", "Quantity > 0"));

            item.HasOne(e => e.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(e => e.OrderId);

            item.HasOne(e => e.Book)
                .WithMany(b => b.OrderItems)
                .HasForeignKey(e => e.BookId);
        });
    }
}