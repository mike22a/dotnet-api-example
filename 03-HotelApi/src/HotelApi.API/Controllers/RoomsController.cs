using HotelApi.Application.Common;
using HotelApi.Application.DTOs.Room;
using HotelApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApi.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RoomsController(RoomService svc) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<RoomResponse>>>> GetAll(
        [FromQuery] int? roomTypeId, [FromQuery] string? status,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var (rooms, total) = await svc.GetAllAsync(roomTypeId, status, page, pageSize);
        Response.Headers.Append("X-Total-Count", total.ToString());
        return Ok(ApiResponse<List<RoomResponse>>.SuccessResult(rooms));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<RoomResponse>>> GetById(int id)
        => Ok(ApiResponse<RoomResponse>.SuccessResult(await svc.GetByIdAsync(id)));

    [HttpGet("available")]
    public async Task<ActionResult<ApiResponse<List<RoomResponse>>>> GetAvailable([FromQuery] AvailableRoomRequest req)
        => Ok(ApiResponse<List<RoomResponse>>.SuccessResult(await svc.GetAvailableAsync(req), "Available rooms retrieved."));

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<RoomResponse>>> Create([FromBody] CreateRoomRequest req)
    {
        var result = await svc.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<RoomResponse>.SuccessResult(result, "Room created."));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<RoomResponse>>> Update(int id, [FromBody] UpdateRoomRequest req)
        => Ok(ApiResponse<RoomResponse>.SuccessResult(await svc.UpdateAsync(id, req), "Room updated."));

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        await svc.DeleteAsync(id);
        return Ok(ApiResponse<string>.SuccessResult("Room deleted."));
    }
}
