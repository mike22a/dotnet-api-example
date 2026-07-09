using HotelApi.Domain.Entities;
namespace HotelApi.Domain.Interfaces.Repositories;

public interface IRoomTypeRepository
{
    Task<List<RoomType>> GetAllAsync();
    Task<RoomType?> FindByIdAsync(int id);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    Task<RoomType> CreateAsync(RoomType roomType);
    Task<RoomType> UpdateAsync(RoomType roomType);
    Task DeleteAsync(RoomType roomType);
}
