using HotelApi.Application.DTOs.Room;
using HotelApi.Domain.Entities;
using HotelApi.Domain.Exceptions;
using HotelApi.Domain.Interfaces.Repositories;

namespace HotelApi.Application.Services;

public class RoomService(IRoomRepository roomRepo, IRoomTypeRepository roomTypeRepo)
{
    public async Task<(List<RoomResponse> Rooms, int Total)> GetAllAsync(int? roomTypeId, string? status, int page, int pageSize)
    {
        RoomStatus? s = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<RoomStatus>(status, true, out var ps)) s = ps;
        var (rooms, total) = await roomRepo.GetAllAsync(roomTypeId, s, page, pageSize);
        return (rooms.Select(Map).ToList(), total);
    }

    public async Task<RoomResponse> GetByIdAsync(int id)
    {
        var room = await roomRepo.FindByIdAsync(id) ?? throw new NotFoundException($"Room {id} not found.");
        return Map(room);
    }

    public async Task<List<RoomResponse>> GetAvailableAsync(AvailableRoomRequest req)
    {
        if (req.CheckOutDate <= req.CheckInDate)
            throw new BusinessRuleException("Check-out date must be after check-in date.");
        var rooms = await roomRepo.GetAvailableRoomsAsync(req.RoomTypeId, req.CheckInDate, req.CheckOutDate);
        return rooms.Select(Map).ToList();
    }

    public async Task<RoomResponse> CreateAsync(CreateRoomRequest req)
    {
        if (await roomRepo.ExistsByRoomNumberAsync(req.RoomNumber))
            throw new ConflictException($"Room number '{req.RoomNumber}' already exists.");
        var roomType = await roomTypeRepo.FindByIdAsync(req.RoomTypeId)
            ?? throw new NotFoundException($"RoomType {req.RoomTypeId} not found.");

        var room = new Room { RoomNumber = req.RoomNumber, Floor = req.Floor, RoomTypeId = req.RoomTypeId };
        return Map(await roomRepo.CreateAsync(room));
    }

    public async Task<RoomResponse> UpdateAsync(int id, UpdateRoomRequest req)
    {
        var room = await roomRepo.FindByIdAsync(id) ?? throw new NotFoundException($"Room {id} not found.");
        if (await roomRepo.ExistsByRoomNumberAsync(req.RoomNumber, excludeId: id))
            throw new ConflictException($"Room number '{req.RoomNumber}' already exists.");
        if (await roomTypeRepo.FindByIdAsync(req.RoomTypeId) == null)
            throw new NotFoundException($"RoomType {req.RoomTypeId} not found.");

        room.RoomNumber = req.RoomNumber; room.Floor = req.Floor;
        room.RoomTypeId = req.RoomTypeId; room.Status = req.Status;
        return Map(await roomRepo.UpdateAsync(room));
    }

    public async Task DeleteAsync(int id)
    {
        var room = await roomRepo.FindByIdAsync(id) ?? throw new NotFoundException($"Room {id} not found.");
        if (room.Status == RoomStatus.Occupied)
            throw new BusinessRuleException("Cannot delete an occupied room.");
        await roomRepo.DeleteAsync(room);
    }

    private static RoomResponse Map(Room r) =>
        new(r.Id, r.RoomNumber, r.Floor, r.RoomTypeId, r.RoomType?.Name ?? "", r.RoomType?.PricePerNight ?? 0, r.Status.ToString(), r.CreatedAt);
}
