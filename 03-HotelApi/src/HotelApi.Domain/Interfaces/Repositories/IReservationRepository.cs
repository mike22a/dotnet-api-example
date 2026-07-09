using HotelApi.Domain.Entities;
namespace HotelApi.Domain.Interfaces.Repositories;

public interface IReservationRepository
{
    Task<(List<Reservation> Items, int Total)> GetAllAsync(ReservationStatus? status, int? guestId, int page, int pageSize);
    Task<Reservation?> FindByIdAsync(int id);
    Task<bool> HasConflictAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeId = null);
    Task<Reservation> CreateAsync(Reservation reservation);
    Task<Reservation> UpdateAsync(Reservation reservation);
    // Reports
    Task<List<Reservation>> GetCheckedOutInRangeAsync(DateTime from, DateTime to);
}
