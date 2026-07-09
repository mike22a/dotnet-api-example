using LibraryApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Borrowing> Borrowings => Set<Borrowing>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique constraints
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Book>()
            .HasIndex(b => b.ISBN)
            .IsUnique();

        // Relationships
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Borrowing>()
            .HasOne(b => b.Book)
            .WithMany()
            .HasForeignKey(b => b.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed data
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Fiction" },
            new Category { Id = 2, Name = "Science & Technology" },
            new Category { Id = 3, Name = "History" },
            new Category { Id = 4, Name = "Biography" }
        );

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "Introduction to Algorithms", Author = "Thomas H. Cormen", ISBN = "9780262033848", CategoryId = 2, TotalCopies = 5, AvailableCopies = 5 },
            new Book { Id = 2, Title = "Clean Code", Author = "Robert C. Martin", ISBN = "9780132350884", CategoryId = 2, TotalCopies = 3, AvailableCopies = 3 },
            new Book { Id = 3, Title = "The Hobbit", Author = "J.R.R. Tolkien", ISBN = "9780261102217", CategoryId = 1, TotalCopies = 8, AvailableCopies = 8 }
        );

        // Seed Admin and Librarian
        // Password for both is 'password123' hashed with BCrypt
        // Hash: $2a$11$k.Ww2Fq3T9.Z1t64v7F2/e1XoH2v4B9T1w.qZ9E1lC/g1vV1b1b1.
        modelBuilder.Entity<User>().HasData(
            new User 
            { 
                Id = 1, 
                Name = "System Admin", 
                Email = "admin@library.com", 
                PasswordHash = "$2a$11$k.Ww2Fq3T9.Z1t64v7F2/e1XoH2v4B9T1w.qZ9E1lC/g1vV1b1b1.", 
                Role = UserRole.Admin,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User 
            { 
                Id = 2, 
                Name = "John Librarian", 
                Email = "john@library.com", 
                PasswordHash = "$2a$11$k.Ww2Fq3T9.Z1t64v7F2/e1XoH2v4B9T1w.qZ9E1lC/g1vV1b1b1.", 
                Role = UserRole.Librarian,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
