using BookstoreApi.Domain.Entities;
using BookstoreApi.Domain.Interfaces.Repositories;
using BookstoreApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookstoreApi.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context) => _context = context;

    public async Task<(List<Book> Books, int Total)> GetAllAsync(
        string? search, int? categoryId, int page, int pageSize)
    {
        var query = _context.Books
            .Include(b => b.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(b =>
                b.Title.Contains(search) ||
                b.Author.Contains(search) ||
                b.ISBN.Contains(search));

        if (categoryId.HasValue)
            query = query.Where(b => b.CategoryId == categoryId.Value);

        var total = await query.CountAsync();

        var books = await query
            .OrderBy(b => b.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (books, total);
    }

    public async Task<Book?> FindByIdAsync(int id) =>
        await _context.Books
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<bool> ExistsByISBNAsync(string isbn, int? excludeId = null)
    {
        var query = _context.Books.Where(b => b.ISBN == isbn);

        if (excludeId.HasValue)
            query = query.Where(b => b.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<Book> CreateAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        await _context.Entry(book).Reference(b => b.Category).LoadAsync();
        return book;
    }

    public async Task<Book> UpdateAsync(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
        await _context.Entry(book).Reference(b => b.Category).LoadAsync();
        return book;
    }

    public async Task DeleteAsync(Book book)
    {
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }
}
