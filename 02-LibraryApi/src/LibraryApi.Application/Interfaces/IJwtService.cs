using LibraryApi.Domain.Entities;

namespace LibraryApi.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
