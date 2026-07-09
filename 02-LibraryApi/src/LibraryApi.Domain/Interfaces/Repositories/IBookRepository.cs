using LibraryApi.Domain.Entities;

namespace LibraryApi.Domain.Interfaces.Repositories;

public interface IBookRepository
{
    Task<(List<Book> Books, int Total)> GetAllAsync(string? search, int? categoryId, int page, int pageSize);
    Task<Book?> FindByIdAsync(int id);
    Task<bool> ExistsByISBNAsync(string isbn, int? excludeId = null);
    Task<Book> CreateAsync(Book book);
    Task<Book> UpdateAsync(Book book);
    Task DeleteAsync(Book book);
}
