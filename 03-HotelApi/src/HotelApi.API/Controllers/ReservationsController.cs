using HotelApi.Application.Common;
using HotelApi.Application.DTOs.Reservation;
using HotelApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelApi.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReservationsController(ReservationService svc) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ReservationResponse>>>> GetAll(
        [FromQuery] string? status, [FromQuery] int? guestId,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var (items, total) = await svc.GetAllAsync(status, guestId, page, pageSize);
        Response.Headers.Append("X-Total-Count", total.ToString());
        return Ok(ApiResponse<List<ReservationResponse>>.SuccessResult(items));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ReservationResponse>>> GetById(int id)
        => Ok(ApiResponse<ReservationResponse>.SuccessResult(await svc.GetByIdAsync(id)));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReservationResponse>>> Create([FromBody] CreateReservationRequest req)
    {
        var uid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await svc.CreateAsync(req, uid);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ReservationResponse>.SuccessResult(result, "Reservation created."));
    }

    [HttpPatch("{id:int}/confirm")]
    public async Task<ActionResult<ApiResponse<ReservationResponse>>> Confirm(int id)
        => Ok(ApiResponse<ReservationResponse>.SuccessResult(await svc.ConfirmAsync(id), "Reservation confirmed."));

    [HttpPatch("{id:int}/check-in")]
    public async Task<ActionResult<ApiResponse<ReservationResponse>>> CheckIn(int id)
        => Ok(ApiResponse<ReservationResponse>.SuccessResult(await svc.CheckInAsync(id), "Guest checked in."));

    [HttpPatch("{id:int}/check-out")]
    public async Task<ActionResult<ApiResponse<ReservationResponse>>> CheckOut(int id)
        => Ok(ApiResponse<ReservationResponse>.SuccessResult(await svc.CheckOutAsync(id), "Guest checked out."));

    [HttpPatch("{id:int}/cancel")]
    public async Task<ActionResult<ApiResponse<ReservationResponse>>> Cancel(int id)
        => Ok(ApiResponse<ReservationResponse>.SuccessResult(await svc.CancelAsync(id), "Reservation cancelled."));
}
