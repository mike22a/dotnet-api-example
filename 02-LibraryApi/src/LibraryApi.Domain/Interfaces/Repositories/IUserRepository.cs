using LibraryApi.Domain.Entities;

namespace LibraryApi.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string email);
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByIdAsync(int id);
    Task<User> CreateAsync(User user, string plainPassword);
    bool VerifyPassword(string plainPassword, string passwordHash);
    Task<List<User>> GetAllLibrariansAsync();
    Task DeleteAsync(User user);
}
