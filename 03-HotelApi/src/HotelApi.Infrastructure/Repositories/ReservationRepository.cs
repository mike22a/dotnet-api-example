using HotelApi.Domain.Entities;
using HotelApi.Domain.Interfaces.Repositories;
using HotelApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelApi.Infrastructure.Repositories;

public class ReservationRepository(AppDbContext ctx) : IReservationRepository
{
    private IQueryable<Reservation> WithIncludes() =>
        ctx.Reservations
            .Include(r => r.Guest)
            .Include(r => r.Room).ThenInclude(rm => rm.RoomType)
            .Include(r => r.Payment);

    public async Task<(List<Reservation> Items, int Total)> GetAllAsync(ReservationStatus? status, int? guestId, int page, int pageSize)
    {
        var q = WithIncludes().AsQueryable();
        if (status.HasValue)   q = q.Where(r => r.Status == status.Value);
        if (guestId.HasValue)  q = q.Where(r => r.GuestId == guestId.Value);
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(r => r.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<Reservation?> FindByIdAsync(int id) =>
        await WithIncludes().FirstOrDefaultAsync(r => r.Id == id);

    public async Task<bool> HasConflictAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeId = null)
    {
        var q = ctx.Reservations.Where(r =>
            r.RoomId == roomId &&
            r.Status != ReservationStatus.Cancelled &&
            r.Status != ReservationStatus.CheckedOut &&
            r.CheckInDate < checkOut && r.CheckOutDate > checkIn);
        if (excludeId.HasValue) q = q.Where(r => r.Id != excludeId.Value);
        return await q.AnyAsync();
    }

    public async Task<Reservation> CreateAsync(Reservation r)
    {
        ctx.Reservations.Add(r);
        await ctx.SaveChangesAsync();
        return (await FindByIdAsync(r.Id))!;
    }

    public async Task<Reservation> UpdateAsync(Reservation r)
    {
        ctx.Reservations.Update(r);
        await ctx.SaveChangesAsync();
        return (await FindByIdAsync(r.Id))!;
    }

    public async Task<List<Reservation>> GetCheckedOutInRangeAsync(DateTime from, DateTime to) =>
        await WithIncludes()
            .Where(r => r.Status == ReservationStatus.CheckedOut &&
                        r.CheckOutDate >= from && r.CheckOutDate <= to)
            .ToListAsync();
}
