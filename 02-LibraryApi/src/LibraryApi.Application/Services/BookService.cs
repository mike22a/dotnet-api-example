using LibraryApi.Application.DTOs.Book;
using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Exceptions;
using LibraryApi.Domain.Interfaces.Repositories;

namespace LibraryApi.Application.Services;

public class BookService
{
    private readonly IBookRepository _bookRepo;
    private readonly ICategoryRepository _categoryRepo;

    public BookService(IBookRepository bookRepo, ICategoryRepository categoryRepo)
    {
        _bookRepo = bookRepo;
        _categoryRepo = categoryRepo;
    }

    public async Task<(List<BookResponse> Books, int Total)> GetAllAsync(string? search, int? categoryId, int page, int pageSize)
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
        if (await _bookRepo.ExistsByISBNAsync(request.ISBN))
            throw new ConflictException($"A book with ISBN '{request.ISBN}' already exists.");

        var category = await _categoryRepo.FindByIdAsync(request.CategoryId)
            ?? throw new NotFoundException($"Category with ID {request.CategoryId} not found.");

        var book = new Book
        {
            Title = request.Title,
            Author = request.Author,
            ISBN = request.ISBN,
            CategoryId = request.CategoryId,
            TotalCopies = request.TotalCopies,
            AvailableCopies = request.TotalCopies // Initially, all copies are available
        };

        var created = await _bookRepo.CreateAsync(book);
        return MapToResponse(created);
    }

    public async Task<BookResponse> UpdateAsync(int id, UpdateBookRequest request)
    {
        var book = await _bookRepo.FindByIdAsync(id)
            ?? throw new NotFoundException($"Book with ID {id} not found.");

        if (await _bookRepo.ExistsByISBNAsync(request.ISBN, excludeId: id))
            throw new ConflictException($"A book with ISBN '{request.ISBN}' already exists.");

        var category = await _categoryRepo.FindByIdAsync(request.CategoryId)
            ?? throw new NotFoundException($"Category with ID {request.CategoryId} not found.");

        // Calculate available copies based on new total
        int activeLoans = book.TotalCopies - book.AvailableCopies;
        if (request.TotalCopies < activeLoans)
            throw new BusinessRuleException($"Cannot reduce TotalCopies below active loans ({activeLoans} copies are currently borrowed).");

        book.Title = request.Title;
        book.Author = request.Author;
        book.ISBN = request.ISBN;
        book.CategoryId = request.CategoryId;
        book.TotalCopies = request.TotalCopies;
        book.AvailableCopies = request.TotalCopies - activeLoans;

        var updated = await _bookRepo.UpdateAsync(book);
        return MapToResponse(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var book = await _bookRepo.FindByIdAsync(id)
            ?? throw new NotFoundException($"Book with ID {id} not found.");

        int activeLoans = book.TotalCopies - book.AvailableCopies;
        if (activeLoans > 0)
            throw new BusinessRuleException($"Cannot delete book '{book.Title}' because it has {activeLoans} active borrowing(s).");

        await _bookRepo.DeleteAsync(book);
    }

    private static BookResponse MapToResponse(Book b) => new()
    {
        Id = b.Id,
        Title = b.Title,
        Author = b.Author,
        ISBN = b.ISBN,
        CategoryId = b.CategoryId,
        CategoryName = b.Category?.Name ?? string.Empty,
        TotalCopies = b.TotalCopies,
        AvailableCopies = b.AvailableCopies,
        CreatedAt = b.CreatedAt
    };
}
