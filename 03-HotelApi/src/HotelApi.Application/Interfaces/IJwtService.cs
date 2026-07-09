using HotelApi.Domain.Entities;
namespace HotelApi.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
