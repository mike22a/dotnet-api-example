using HotelApi.Application.Common;
using HotelApi.Application.DTOs.Guest;
using HotelApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApi.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GuestsController(GuestService svc) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<GuestResponse>>>> GetAll(
        [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var (guests, total) = await svc.GetAllAsync(search, page, pageSize);
        Response.Headers.Append("X-Total-Count", total.ToString());
        return Ok(ApiResponse<List<GuestResponse>>.SuccessResult(guests));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<GuestResponse>>> GetById(int id)
        => Ok(ApiResponse<GuestResponse>.SuccessResult(await svc.GetByIdAsync(id)));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<GuestResponse>>> Create([FromBody] CreateGuestRequest req)
    {
        var result = await svc.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<GuestResponse>.SuccessResult(result, "Guest registered."));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<GuestResponse>>> Update(int id, [FromBody] UpdateGuestRequest req)
        => Ok(ApiResponse<GuestResponse>.SuccessResult(await svc.UpdateAsync(id, req), "Guest updated."));

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        await svc.DeleteAsync(id);
        return Ok(ApiResponse<string>.SuccessResult("Guest deleted."));
    }
}
