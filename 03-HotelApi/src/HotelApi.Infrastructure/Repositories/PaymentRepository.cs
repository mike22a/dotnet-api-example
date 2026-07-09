using HotelApi.Domain.Entities;
using HotelApi.Domain.Interfaces.Repositories;
using HotelApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelApi.Infrastructure.Repositories;

public class PaymentRepository(AppDbContext ctx) : IPaymentRepository
{
    public async Task<Payment?> FindByReservationIdAsync(int reservationId) =>
        await ctx.Payments.FirstOrDefaultAsync(p => p.ReservationId == reservationId);

    public async Task<Payment> CreateAsync(Payment p) { ctx.Payments.Add(p);    await ctx.SaveChangesAsync(); return p; }
    public async Task<Payment> UpdateAsync(Payment p) { ctx.Payments.Update(p); await ctx.SaveChangesAsync(); return p; }
}
