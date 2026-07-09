using HotelApi.Application.DTOs.Auth;
using HotelApi.Application.Interfaces;
using HotelApi.Domain.Entities;
using HotelApi.Domain.Exceptions;
using HotelApi.Domain.Interfaces.Repositories;

namespace HotelApi.Application.Services;

public class AuthService(IUserRepository repo, IJwtService jwt)
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
    {
        if (await repo.ExistsByEmailAsync(req.Email))
            throw new ConflictException($"Email '{req.Email}' is already registered.");

        var user = new User { Name = req.Name, Email = req.Email, Role = req.Role };
        var created = await repo.CreateAsync(user, req.Password);
        return new AuthResponse(jwt.GenerateToken(created), created.Name, created.Email, created.Role.ToString());
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest req)
    {
        var user = await repo.FindByEmailAsync(req.Email)
            ?? throw new NotFoundException("Invalid email or password.");

        if (!repo.VerifyPassword(req.Password, user.PasswordHash))
            throw new NotFoundException("Invalid email or password.");

        return new AuthResponse(jwt.GenerateToken(user), user.Name, user.Email, user.Role.ToString());
    }

    public async Task<UserResponse> GetProfileAsync(int userId)
    {
        var u = await repo.FindByIdAsync(userId)
            ?? throw new NotFoundException($"User {userId} not found.");
        return new UserResponse(u.Id, u.Name, u.Email, u.Role.ToString(), u.CreatedAt);
    }
}
