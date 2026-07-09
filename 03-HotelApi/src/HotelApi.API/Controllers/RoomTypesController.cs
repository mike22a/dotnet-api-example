using HotelApi.Application.Common;
using HotelApi.Application.DTOs.RoomType;
using HotelApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApi.API.Controllers;

[Authorize]
[ApiController]
[Route("api/room-types")]
public class RoomTypesController(RoomTypeService svc) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<RoomTypeResponse>>>> GetAll()
        => Ok(ApiResponse<List<RoomTypeResponse>>.SuccessResult(await svc.GetAllAsync()));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<RoomTypeResponse>>> GetById(int id)
        => Ok(ApiResponse<RoomTypeResponse>.SuccessResult(await svc.GetByIdAsync(id)));

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<RoomTypeResponse>>> Create([FromBody] CreateRoomTypeRequest req)
    {
        var result = await svc.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<RoomTypeResponse>.SuccessResult(result, "Room type created."));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<RoomTypeResponse>>> Update(int id, [FromBody] UpdateRoomTypeRequest req)
        => Ok(ApiResponse<RoomTypeResponse>.SuccessResult(await svc.UpdateAsync(id, req), "Room type updated."));

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        await svc.DeleteAsync(id);
        return Ok(ApiResponse<string>.SuccessResult("Room type deleted."));
    }
}
