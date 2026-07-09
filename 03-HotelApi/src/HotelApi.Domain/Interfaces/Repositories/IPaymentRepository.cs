using HotelApi.Domain.Entities;
namespace HotelApi.Domain.Interfaces.Repositories;

public interface IPaymentRepository
{
    Task<Payment?> FindByReservationIdAsync(int reservationId);
    Task<Payment> CreateAsync(Payment payment);
    Task<Payment> UpdateAsync(Payment payment);
}
