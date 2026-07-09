using HotelApi.Application.DTOs.Guest;
using HotelApi.Domain.Entities;
using HotelApi.Domain.Exceptions;
using HotelApi.Domain.Interfaces.Repositories;

namespace HotelApi.Application.Services;

public class GuestService(IGuestRepository repo)
{
    public async Task<(List<GuestResponse> Guests, int Total)> GetAllAsync(string? search, int page, int pageSize)
    {
        var (guests, total) = await repo.GetAllAsync(search, page, pageSize);
        return (guests.Select(Map).ToList(), total);
    }

    public async Task<GuestResponse> GetByIdAsync(int id)
    {
        var g = await repo.FindByIdAsync(id) ?? throw new NotFoundException($"Guest {id} not found.");
        return Map(g);
    }

    public async Task<GuestResponse> CreateAsync(CreateGuestRequest req)
    {
        if (await repo.ExistsByEmailAsync(req.Email))
            throw new ConflictException($"A guest with email '{req.Email}' already exists.");
        var guest = new Guest { FullName = req.FullName, Email = req.Email, Phone = req.Phone, IdentityNumber = req.IdentityNumber, Nationality = req.Nationality };
        return Map(await repo.CreateAsync(guest));
    }

    public async Task<GuestResponse> UpdateAsync(int id, UpdateGuestRequest req)
    {
        var g = await repo.FindByIdAsync(id) ?? throw new NotFoundException($"Guest {id} not found.");
        if (await repo.ExistsByEmailAsync(req.Email, excludeId: id))
            throw new ConflictException($"A guest with email '{req.Email}' already exists.");
        g.FullName = req.FullName; g.Email = req.Email; g.Phone = req.Phone;
        g.IdentityNumber = req.IdentityNumber; g.Nationality = req.Nationality;
        return Map(await repo.UpdateAsync(g));
    }

    public async Task DeleteAsync(int id)
    {
        var g = await repo.FindByIdAsync(id) ?? throw new NotFoundException($"Guest {id} not found.");
        await repo.DeleteAsync(g);
    }

    private static GuestResponse Map(Guest g) =>
        new(g.Id, g.FullName, g.Email, g.Phone, g.IdentityNumber, g.Nationality, g.CreatedAt);
}
