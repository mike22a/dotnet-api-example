using BookstoreApi.Domain.Entities;

namespace BookstoreApi.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string email);
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByIdAsync(int id);
    /// <summary>Creates the user, hashing the password before saving.</summary>
    Task<User> CreateAsync(User user, string plainPassword);
    /// <summary>Verifies a plain text password against the stored hash.</summary>
    bool VerifyPassword(string plainPassword, string passwordHash);
}
