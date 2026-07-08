using BookstoreApi.Application.Data;
using BookstoreApi.Application.DTOs.Book;
using BookstoreApi.Domain.Entities;
using BookstoreApi.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookstoreApi.Application.Services;

public class BookService
{
    private readonly AppDbContext _context;

    public BookService(AppDbContext context) => _context = context;

    public async Task<(List<BookResponse> Books, int Total)> GetAllAsync(
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
            .Select(b => new BookResponse
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                ISBN = b.ISBN,
                Price = b.Price,
                Stock = b.Stock,
                CategoryId = b.CategoryId,
                CategoryName = b.Category.Name,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync();

        return (books, total);
    }

    public async Task<BookResponse> GetByIdAsync(int id)
    {
        var book = await _context.Books
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id)
            ?? throw new NotFoundException($"Book with ID {id} not found.");

        return MapToResponse(book);
    }

    public async Task<BookResponse> CreateAsync(CreateBookRequest request)
    {
        // Business rule: ISBN must be unique
        var isbnExists = await _context.Books.AnyAsync(b => b.ISBN == request.ISBN);
        if (isbnExists)
            throw new ConflictException($"A book with ISBN '{request.ISBN}' already exists.");

        // Validate category exists
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            throw new NotFoundException($"Category with ID {request.CategoryId} not found.");

        var book = new Book
        {
            Title = request.Title,
            Author = request.Author,
            ISBN = request.ISBN,
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(book.Id);
    }

    public async Task<BookResponse> UpdateAsync(int id, UpdateBookRequest request)
    {
        var book = await _context.Books.FindAsync(id)
            ?? throw new NotFoundException($"Book with ID {id} not found.");

        // Business rule: ISBN must be unique (excluding current book)
        var isbnExists = await _context.Books
            .AnyAsync(b => b.ISBN == request.ISBN && b.Id != id);
        if (isbnExists)
            throw new ConflictException($"A book with ISBN '{request.ISBN}' already exists.");

        // Validate category exists
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            throw new NotFoundException($"Category with ID {request.CategoryId} not found.");

        book.Title = request.Title;
        book.Author = request.Author;
        book.ISBN = request.ISBN;
        book.Price = request.Price;
        book.Stock = request.Stock;
        book.CategoryId = request.CategoryId;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var book = await _context.Books.FindAsync(id)
            ?? throw new NotFoundException($"Book with ID {id} not found.");

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }

    private static BookResponse MapToResponse(Book book) => new()
    {
        Id = book.Id,
        Title = book.Title,
        Author = book.Author,
        ISBN = book.ISBN,
        Price = book.Price,
        Stock = book.Stock,
        CategoryId = book.CategoryId,
        CategoryName = book.Category?.Name ?? string.Empty,
        CreatedAt = book.CreatedAt
    };
}
