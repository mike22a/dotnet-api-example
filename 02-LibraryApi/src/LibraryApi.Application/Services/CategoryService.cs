using LibraryApi.Application.DTOs.Category;
using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Exceptions;
using LibraryApi.Domain.Interfaces.Repositories;

namespace LibraryApi.Application.Services;

public class CategoryService
{
    private readonly ICategoryRepository _categoryRepo;

    public CategoryService(ICategoryRepository categoryRepo) => _categoryRepo = categoryRepo;

    public async Task<List<CategoryResponse>> GetAllAsync()
    {
        var categories = await _categoryRepo.GetAllAsync();
        return categories.Select(MapToResponse).ToList();
    }

    public async Task<CategoryResponse> GetByIdAsync(int id)
    {
        var category = await _categoryRepo.FindByIdAsync(id, includeBooks: true)
            ?? throw new NotFoundException($"Category with ID {id} not found.");

        return MapToResponse(category);
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        var exists = await _categoryRepo.ExistsByNameAsync(request.Name);
        if (exists)
            throw new ConflictException($"Category '{request.Name}' already exists.");

        var category = new Category { Name = request.Name };
        var created = await _categoryRepo.CreateAsync(category);

        return MapToResponse(created);
    }

    public async Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        var category = await _categoryRepo.FindByIdAsync(id)
            ?? throw new NotFoundException($"Category with ID {id} not found.");

        var nameConflict = await _categoryRepo.ExistsByNameAsync(request.Name, excludeId: id);
        if (nameConflict)
            throw new ConflictException($"Category '{request.Name}' already exists.");

        category.Name = request.Name;
        var updated = await _categoryRepo.UpdateAsync(category);

        return MapToResponse(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _categoryRepo.FindByIdAsync(id, includeBooks: true)
            ?? throw new NotFoundException($"Category with ID {id} not found.");

        if (category.Books.Count > 0)
            throw new BusinessRuleException($"Cannot delete category '{category.Name}' because it has {category.Books.Count} book(s) associated with it.");

        await _categoryRepo.DeleteAsync(category);
    }

    private static CategoryResponse MapToResponse(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        BookCount = c.Books.Count
    };
}
