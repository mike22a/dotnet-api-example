using HotelApi.Domain.Entities;
namespace HotelApi.Domain.Interfaces.Repositories;

public interface IGuestRepository
{
    Task<(List<Guest> Guests, int Total)> GetAllAsync(string? search, int page, int pageSize);
    Task<Guest?> FindByIdAsync(int id);
    Task<bool> ExistsByEmailAsync(string email, int? excludeId = null);
    Task<Guest> CreateAsync(Guest guest);
    Task<Guest> UpdateAsync(Guest guest);
    Task DeleteAsync(Guest guest);
}
