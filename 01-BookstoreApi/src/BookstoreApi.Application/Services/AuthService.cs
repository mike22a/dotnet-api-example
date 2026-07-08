using BookstoreApi.Application.Data;
using BookstoreApi.Application.DTOs.Auth;
using BookstoreApi.Domain.Entities;
using BookstoreApi.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookstoreApi.Application.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AuthService(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var exists = await _context.Users.AnyAsync(u => u.Email == request.Email);
        if (exists)
            throw new ConflictException($"Email '{request.Email}' is already registered.");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Token = _jwtService.GenerateToken(user),
            Name = user.Name,
            Email = user.Email
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email)
            ?? throw new NotFoundException("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
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
        var user = await _context.Users.FindAsync(userId)
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
