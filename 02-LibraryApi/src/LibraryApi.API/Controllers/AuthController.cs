using LibraryApi.Application.Common;
using LibraryApi.Application.DTOs.Auth;
using LibraryApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService) => _authService = authService;

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(ApiResponse<AuthResponse>.SuccessResult(result, "Registration successful."));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(ApiResponse<AuthResponse>.SuccessResult(result, "Login successful."));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return Unauthorized(ApiResponse<UserResponse>.FailResult("Unauthorized"));

        var profile = await _authService.GetProfileAsync(userId);
        return Ok(ApiResponse<UserResponse>.SuccessResult(profile, "Profile retrieved successfully."));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("librarians")]
    public async Task<ActionResult<ApiResponse<List<UserResponse>>>> GetLibrarians()
    {
        var librarians = await _authService.GetLibrariansAsync();
        return Ok(ApiResponse<List<UserResponse>>.SuccessResult(librarians, "Librarians list retrieved successfully."));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("librarians/{id:int}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteLibrarian(int id)
    {
        await _authService.DeleteLibrarianAsync(id);
        return Ok(ApiResponse<string>.SuccessResult("Librarian deleted successfully."));
    }
}
