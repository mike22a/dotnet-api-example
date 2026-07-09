using HotelApi.Domain.Entities;
using HotelApi.Domain.Interfaces.Repositories;
using HotelApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelApi.Infrastructure.Repositories;

public class RoomTypeRepository(AppDbContext ctx) : IRoomTypeRepository
{
    public async Task<List<RoomType>> GetAllAsync() =>
        await ctx.RoomTypes.Include(r => r.Rooms).ToListAsync();

    public async Task<RoomType?> FindByIdAsync(int id) =>
        await ctx.RoomTypes.Include(r => r.Rooms).FirstOrDefaultAsync(r => r.Id == id);

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        var q = ctx.RoomTypes.Where(r => r.Name.ToLower() == name.ToLower());
        if (excludeId.HasValue) q = q.Where(r => r.Id != excludeId.Value);
        return await q.AnyAsync();
    }

    public async Task<RoomType> CreateAsync(RoomType rt)  { ctx.RoomTypes.Add(rt);    await ctx.SaveChangesAsync(); return rt; }
    public async Task<RoomType> UpdateAsync(RoomType rt)  { ctx.RoomTypes.Update(rt); await ctx.SaveChangesAsync(); return rt; }
    public async Task DeleteAsync(RoomType rt)             { ctx.RoomTypes.Remove(rt); await ctx.SaveChangesAsync(); }
}
