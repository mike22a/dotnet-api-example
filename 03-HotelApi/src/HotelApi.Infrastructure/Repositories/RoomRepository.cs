using HotelApi.Domain.Entities;
using HotelApi.Domain.Interfaces.Repositories;
using HotelApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelApi.Infrastructure.Repositories;

public class RoomRepository(AppDbContext ctx) : IRoomRepository
{
    public async Task<(List<Room> Rooms, int Total)> GetAllAsync(int? roomTypeId, RoomStatus? status, int page, int pageSize)
    {
        var q = ctx.Rooms.Include(r => r.RoomType).AsQueryable();
        if (roomTypeId.HasValue) q = q.Where(r => r.RoomTypeId == roomTypeId.Value);
        if (status.HasValue)     q = q.Where(r => r.Status == status.Value);
        var total = await q.CountAsync();
        var rooms = await q.OrderBy(r => r.RoomNumber).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (rooms, total);
    }

    public async Task<Room?> FindByIdAsync(int id) =>
        await ctx.Rooms.Include(r => r.RoomType).FirstOrDefaultAsync(r => r.Id == id);

    public async Task<bool> ExistsByRoomNumberAsync(string num, int? excludeId = null)
    {
        var q = ctx.Rooms.Where(r => r.RoomNumber == num);
        if (excludeId.HasValue) q = q.Where(r => r.Id != excludeId.Value);
        return await q.AnyAsync();
    }

    // Available rooms: same room type, not under maintenance, no active conflicting reservations
    public async Task<List<Room>> GetAvailableRoomsAsync(int roomTypeId, DateTime checkIn, DateTime checkOut)
    {
        var bookedRoomIds = await ctx.Reservations
            .Where(r => r.RoomId != 0 &&
                        r.Status != ReservationStatus.Cancelled &&
                        r.Status != ReservationStatus.CheckedOut &&
                        r.CheckInDate < checkOut && r.CheckOutDate > checkIn)
            .Select(r => r.RoomId)
            .ToListAsync();

        return await ctx.Rooms
            .Include(r => r.RoomType)
            .Where(r => r.RoomTypeId == roomTypeId &&
                        r.Status == RoomStatus.Available &&
                        !bookedRoomIds.Contains(r.Id))
            .ToListAsync();
    }

    public async Task<Room> CreateAsync(Room room) { ctx.Rooms.Add(room);    await ctx.SaveChangesAsync(); await ctx.Entry(room).Reference(r => r.RoomType).LoadAsync(); return room; }
    public async Task<Room> UpdateAsync(Room room) { ctx.Rooms.Update(room); await ctx.SaveChangesAsync(); await ctx.Entry(room).Reference(r => r.RoomType).LoadAsync(); return room; }
    public async Task DeleteAsync(Room room)        { ctx.Rooms.Remove(room); await ctx.SaveChangesAsync(); }
}
