using HotelApi.Application.DTOs.RoomType;
using HotelApi.Domain.Entities;
using HotelApi.Domain.Exceptions;
using HotelApi.Domain.Interfaces.Repositories;

namespace HotelApi.Application.Services;

public class RoomTypeService(IRoomTypeRepository repo)
{
    public async Task<List<RoomTypeResponse>> GetAllAsync()
    {
        var list = await repo.GetAllAsync();
        return list.Select(Map).ToList();
    }

    public async Task<RoomTypeResponse> GetByIdAsync(int id)
    {
        var rt = await repo.FindByIdAsync(id) ?? throw new NotFoundException($"RoomType {id} not found.");
        return Map(rt);
    }

    public async Task<RoomTypeResponse> CreateAsync(CreateRoomTypeRequest req)
    {
        if (await repo.ExistsByNameAsync(req.Name))
            throw new ConflictException($"Room type '{req.Name}' already exists.");

        var rt = new RoomType { Name = req.Name, Description = req.Description, PricePerNight = req.PricePerNight, MaxOccupancy = req.MaxOccupancy };
        return Map(await repo.CreateAsync(rt));
    }

    public async Task<RoomTypeResponse> UpdateAsync(int id, UpdateRoomTypeRequest req)
    {
        var rt = await repo.FindByIdAsync(id) ?? throw new NotFoundException($"RoomType {id} not found.");
        if (await repo.ExistsByNameAsync(req.Name, excludeId: id))
            throw new ConflictException($"Room type '{req.Name}' already exists.");

        rt.Name = req.Name; rt.Description = req.Description;
        rt.PricePerNight = req.PricePerNight; rt.MaxOccupancy = req.MaxOccupancy;
        return Map(await repo.UpdateAsync(rt));
    }

    public async Task DeleteAsync(int id)
    {
        var rt = await repo.FindByIdAsync(id) ?? throw new NotFoundException($"RoomType {id} not found.");
        if (rt.Rooms.Count > 0)
            throw new BusinessRuleException($"Cannot delete room type '{rt.Name}' — it has {rt.Rooms.Count} room(s) assigned.");
        await repo.DeleteAsync(rt);
    }

    private static RoomTypeResponse Map(RoomType r) =>
        new(r.Id, r.Name, r.Description, r.PricePerNight, r.MaxOccupancy, r.Rooms.Count);
}
