using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Interfaces.Repositories;
using LibraryApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    public async Task<bool> ExistsByEmailAsync(string email) =>
        await _context.Users.AnyAsync(u => u.Email == email);

    public async Task<User?> FindByEmailAsync(string email) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> FindByIdAsync(int id) =>
        await _context.Users.FindAsync(id);

    public async Task<User> CreateAsync(User user, string plainPassword)
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public bool VerifyPassword(string plainPassword, string passwordHash) =>
        BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);

    public async Task<List<User>> GetAllLibrariansAsync() =>
        await _context.Users
            .Where(u => u.Role == UserRole.Librarian)
            .OrderBy(u => u.Name)
            .ToListAsync();

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}
