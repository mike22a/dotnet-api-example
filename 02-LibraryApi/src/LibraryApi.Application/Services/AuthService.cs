using LibraryApi.Application.DTOs.Auth;
using LibraryApi.Application.Interfaces;
using LibraryApi.Domain.Entities;
using LibraryApi.Domain.Exceptions;
using LibraryApi.Domain.Interfaces.Repositories;

namespace LibraryApi.Application.Services;

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
        var exists = await _userRepo.ExistsByEmailAsync(request.Email);
        if (exists)
            throw new ConflictException($"Email '{request.Email}' is already registered.");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Role = request.Role
        };

        var created = await _userRepo.CreateAsync(user, request.Password);

        return new AuthResponse
        {
            Token = _jwtService.GenerateToken(created),
            Name = created.Name,
            Email = created.Email,
            Role = created.Role.ToString()
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepo.FindByEmailAsync(request.Email)
            ?? throw new NotFoundException("Invalid email or password.");

        if (!_userRepo.VerifyPassword(request.Password, user.PasswordHash))
            throw new NotFoundException("Invalid email or password.");

        return new AuthResponse
        {
            Token = _jwtService.GenerateToken(user),
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString()
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
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<List<UserResponse>> GetLibrariansAsync()
    {
        var librarians = await _userRepo.GetAllLibrariansAsync();
        return librarians.Select(u => new UserResponse
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Role = u.Role.ToString(),
            CreatedAt = u.CreatedAt
        }).ToList();
    }

    public async Task DeleteLibrarianAsync(int id)
    {
        var user = await _userRepo.FindByIdAsync(id)
            ?? throw new NotFoundException($"Librarian with ID {id} not found.");

        if (user.Role != UserRole.Librarian)
            throw new BusinessRuleException("Only accounts with role Librarian can be deleted.");

        await _userRepo.DeleteAsync(user);
    }
}
