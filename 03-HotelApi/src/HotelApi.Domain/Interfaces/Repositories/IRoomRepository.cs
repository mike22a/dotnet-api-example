using HotelApi.Domain.Entities;
namespace HotelApi.Domain.Interfaces.Repositories;

public interface IRoomRepository
{
    Task<(List<Room> Rooms, int Total)> GetAllAsync(int? roomTypeId, RoomStatus? status, int page, int pageSize);
    Task<Room?> FindByIdAsync(int id);
    Task<bool> ExistsByRoomNumberAsync(string roomNumber, int? excludeId = null);
    Task<List<Room>> GetAvailableRoomsAsync(int roomTypeId, DateTime checkIn, DateTime checkOut);
    Task<Room> CreateAsync(Room room);
    Task<Room> UpdateAsync(Room room);
    Task DeleteAsync(Room room);
}
