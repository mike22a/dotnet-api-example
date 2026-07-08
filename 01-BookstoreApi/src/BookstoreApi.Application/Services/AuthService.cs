using BookstoreApi.Application.DTOs.Auth;
using BookstoreApi.Application.Interfaces;
using BookstoreApi.Domain.Entities;
using BookstoreApi.Domain.Exceptions;
using BookstoreApi.Domain.Interfaces.Repositories;

namespace BookstoreApi.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepo, IJwtService jwtService)
    {
        _userRepo = userRepo;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Business rule: email must be unique
        var exists = await _userRepo.ExistsByEmailAsync(request.Email);
        if (exists)
            throw new ConflictException($"Email '{request.Email}' is already registered.");

        var user = new User { Name = request.Name, Email = request.Email };

        // Password hashing is an infrastructure concern — repository handles it
        var created = await _userRepo.CreateAsync(user, request.Password);

        return new AuthResponse
        {
            Token = _jwtService.GenerateToken(created),
            Name = created.Name,
            Email = created.Email
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepo.FindByEmailAsync(request.Email)
            ?? throw new NotFoundException("Invalid email or password.");

        // Password verification delegated to repository (Infrastructure concern)
        if (!_userRepo.VerifyPassword(request.Password, user.PasswordHash))
            throw new NotFoundException("Invalid email or password.");

        return new AuthResponse
        {
            Token = _jwtService.GenerateToken(user),
            Name = user.Name,
            Email = user.Email
        };
    }

    public async Task<UserResponse> GetProfileAsync(int userId)
    {
        var user = await _userRepo.FindByIdAsync(userId)
            ?? throw new NotFoundException($"User with ID {userId} not found.");

        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }
}
