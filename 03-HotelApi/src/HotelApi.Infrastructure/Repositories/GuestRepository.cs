using HotelApi.Domain.Entities;
using HotelApi.Domain.Interfaces.Repositories;
using HotelApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelApi.Infrastructure.Repositories;

public class GuestRepository(AppDbContext ctx) : IGuestRepository
{
    public async Task<(List<Guest> Guests, int Total)> GetAllAsync(string? search, int page, int pageSize)
    {
        var q = ctx.Guests.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(g => g.FullName.Contains(search) || g.Email.Contains(search) || g.IdentityNumber.Contains(search));
        var total = await q.CountAsync();
        var guests = await q.OrderBy(g => g.FullName).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (guests, total);
    }

    public async Task<Guest?> FindByIdAsync(int id) => await ctx.Guests.FindAsync(id);

    public async Task<bool> ExistsByEmailAsync(string email, int? excludeId = null)
    {
        var q = ctx.Guests.Where(g => g.Email == email);
        if (excludeId.HasValue) q = q.Where(g => g.Id != excludeId.Value);
        return await q.AnyAsync();
    }

    public async Task<Guest> CreateAsync(Guest g) { ctx.Guests.Add(g);    await ctx.SaveChangesAsync(); return g; }
    public async Task<Guest> UpdateAsync(Guest g) { ctx.Guests.Update(g); await ctx.SaveChangesAsync(); return g; }
    public async Task DeleteAsync(Guest g)         { ctx.Guests.Remove(g); await ctx.SaveChangesAsync(); }
}
