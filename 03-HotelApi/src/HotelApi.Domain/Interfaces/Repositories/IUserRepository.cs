using HotelApi.Domain.Entities;
namespace HotelApi.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string email);
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByIdAsync(int id);
    Task<User> CreateAsync(User user, string plainPassword);
    bool VerifyPassword(string plain, string hash);
}
