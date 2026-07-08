using BookstoreApi.Domain.Entities;

namespace BookstoreApi.Application.Interfaces;

/// <summary>
/// Contract for JWT token generation. Implemented in Infrastructure.
/// This interface allows Application services to generate tokens
/// without depending on the JWT implementation details.
/// </summary>
public interface IJwtService
{
    string GenerateToken(User user);
}
