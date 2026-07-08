using BookstoreApi.Application.Data;
using BookstoreApi.Application.DTOs.Category;
using BookstoreApi.Domain.Entities;
using BookstoreApi.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookstoreApi.Application.Services;

public class CategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context) => _context = context;

    public async Task<List<CategoryResponse>> GetAllAsync()
    {
        return await _context.Categories
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                BookCount = c.Books.Count
            })
            .ToListAsync();
    }

    public async Task<CategoryResponse> GetByIdAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Books)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new NotFoundException($"Category with ID {id} not found.");

        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            BookCount = category.Books.Count
        };
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        var exists = await _context.Categories
            .AnyAsync(c => c.Name.ToLower() == request.Name.ToLower());
        if (exists)
            throw new ConflictException($"Category '{request.Name}' already exists.");

        var category = new Category { Name = request.Name };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return new CategoryResponse { Id = category.Id, Name = category.Name, BookCount = 0 };
    }

    public async Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        var category = await _context.Categories.FindAsync(id)
            ?? throw new NotFoundException($"Category with ID {id} not found.");

        var exists = await _context.Categories
            .AnyAsync(c => c.Name.ToLower() == request.Name.ToLower() && c.Id != id);
        if (exists)
            throw new ConflictException($"Category '{request.Name}' already exists.");

        category.Name = request.Name;
        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Books)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new NotFoundException($"Category with ID {id} not found.");

        if (category.Books.Count > 0)
            throw new BusinessRuleException(
                $"Cannot delete category '{category.Name}' because it has {category.Books.Count} book(s). Remove or reassign the books first.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
}
