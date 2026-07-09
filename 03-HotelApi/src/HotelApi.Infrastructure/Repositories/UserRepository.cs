using HotelApi.Domain.Entities;
using HotelApi.Domain.Interfaces.Repositories;
using HotelApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelApi.Infrastructure.Repositories;

public class UserRepository(AppDbContext ctx) : IUserRepository
{
    public async Task<bool> ExistsByEmailAsync(string email) => await ctx.Users.AnyAsync(u => u.Email == email);
    public async Task<User?> FindByEmailAsync(string email) => await ctx.Users.FirstOrDefaultAsync(u => u.Email == email);
    public async Task<User?> FindByIdAsync(int id) => await ctx.Users.FindAsync(id);
    public async Task<User> CreateAsync(User user, string plain)
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plain);
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();
        return user;
    }
    public bool VerifyPassword(string plain, string hash) => BCrypt.Net.BCrypt.Verify(plain, hash);
}
