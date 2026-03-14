namespace exam_Ef_dapper_14_3.data;

using exam_Ef_dapper_14_3.models;
using Microsoft.EntityFrameworkCore;

public class BookShopDbContext : DbContext
{
    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(@"Server=DESKTOP-T32J9LF\SQLEXPRESS;Database=BookShopDb;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(Author =>
        {
            Author.Property(e => e.FullName).HasMaxLength(100).IsRequired();
            Author.HasIndex(e => e.FullName).IsUnique();
        });

        modelBuilder.Entity<Book>(Book =>
        {
            Book.Property(e => e.Title).HasMaxLength(200).IsRequired();
            Book.HasIndex(e => e.Title);
            Book.Property(e => e.Price).HasColumnType("decimal(18,2)");

            Book.ToTable(t => t.HasCheckConstraint("CK_Book_Price", "Price > 0"));
            Book.ToTable(t => t.HasCheckConstraint("CK_Book_StockQuantity", "StockQuantity >= 0"));

            Book.HasOne(e => e.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Order>(Order =>
        {
            Order.Property(e => e.CustomerEmail).IsRequired();
            Order.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<OrderItem>(OrderItem =>
        {
            OrderItem.HasKey(e => new { e.OrderId, e.BookId });
            OrderItem.ToTable(t => t.HasCheckConstraint("CK_OrderItem_Quantity", "Quantity > 0"));

            OrderItem.HasOne(e => e.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(e => e.OrderId);

            OrderItem.HasOne(e => e.Book)
                .WithMany(b => b.OrderItems)
                .HasForeignKey(e => e.BookId);
        });
    }
}
