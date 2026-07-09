using HotelApi.Application.Common;
using HotelApi.Application.DTOs.Auth;
using HotelApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService svc) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest req)
        => Ok(ApiResponse<AuthResponse>.SuccessResult(await svc.RegisterAsync(req), "Registration successful."));

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest req)
        => Ok(ApiResponse<AuthResponse>.SuccessResult(await svc.LoginAsync(req), "Login successful."));

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> Me()
    {
        var uid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(ApiResponse<UserResponse>.SuccessResult(await svc.GetProfileAsync(uid)));
    }
}
