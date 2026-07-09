using LibraryApi.Domain.Entities;

namespace LibraryApi.Domain.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> FindByIdAsync(int id, bool includeBooks = false);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    Task<Category> CreateAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task DeleteAsync(Category category);
}
