using BookstoreApi.Application.DTOs.Book;
using BookstoreApi.Domain.Entities;
using BookstoreApi.Domain.Exceptions;
using BookstoreApi.Domain.Interfaces.Repositories;

namespace BookstoreApi.Application.Services;

public class BookService
{
    private readonly IBookRepository _bookRepo;
    private readonly ICategoryRepository _categoryRepo;

    public BookService(IBookRepository bookRepo, ICategoryRepository categoryRepo)
    {
        _bookRepo = bookRepo;
        _categoryRepo = categoryRepo;
    }

    public async Task<(List<BookResponse> Books, int Total)> GetAllAsync(
        string? search, int? categoryId, int page, int pageSize)
    {
        var (books, total) = await _bookRepo.GetAllAsync(search, categoryId, page, pageSize);
        return (books.Select(MapToResponse).ToList(), total);
    }

    public async Task<BookResponse> GetByIdAsync(int id)
    {
        var book = await _bookRepo.FindByIdAsync(id)
            ?? throw new NotFoundException($"Book with ID {id} not found.");

        return MapToResponse(book);
    }

    public async Task<BookResponse> CreateAsync(CreateBookRequest request)
    {
        // Business rule: ISBN must be unique
        if (await _bookRepo.ExistsByISBNAsync(request.ISBN))
            throw new ConflictException($"A book with ISBN '{request.ISBN}' already exists.");

        // Business rule: Category must exist
        var categoryExists = await _categoryRepo.FindByIdAsync(request.CategoryId);
        if (categoryExists is null)
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

        var created = await _bookRepo.CreateAsync(book);
        return MapToResponse(created);
    }

    public async Task<BookResponse> UpdateAsync(int id, UpdateBookRequest request)
    {
        var book = await _bookRepo.FindByIdAsync(id)
            ?? throw new NotFoundException($"Book with ID {id} not found.");

        // Business rule: ISBN must be unique (excluding current book)
        if (await _bookRepo.ExistsByISBNAsync(request.ISBN, excludeId: id))
            throw new ConflictException($"A book with ISBN '{request.ISBN}' already exists.");

        // Business rule: Category must exist
        var categoryExists = await _categoryRepo.FindByIdAsync(request.CategoryId);
        if (categoryExists is null)
            throw new NotFoundException($"Category with ID {request.CategoryId} not found.");

        book.Title = request.Title;
        book.Author = request.Author;
        book.ISBN = request.ISBN;
        book.Price = request.Price;
        book.Stock = request.Stock;
        book.CategoryId = request.CategoryId;

        var updated = await _bookRepo.UpdateAsync(book);
        return MapToResponse(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var book = await _bookRepo.FindByIdAsync(id)
            ?? throw new NotFoundException($"Book with ID {id} not found.");

        await _bookRepo.DeleteAsync(book);
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
