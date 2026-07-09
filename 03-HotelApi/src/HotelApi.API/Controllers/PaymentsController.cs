using HotelApi.Application.Common;
using HotelApi.Application.DTOs.Payment;
using HotelApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApi.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PaymentsController(PaymentService svc) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PaymentResponse>>> Create([FromBody] CreatePaymentRequest req)
        => Ok(ApiResponse<PaymentResponse>.SuccessResult(await svc.CreateAsync(req), "Payment recorded successfully."));

    [HttpGet("reservation/{reservationId:int}")]
    public async Task<ActionResult<ApiResponse<PaymentResponse>>> GetByReservation(int reservationId)
        => Ok(ApiResponse<PaymentResponse>.SuccessResult(await svc.GetByReservationAsync(reservationId)));
}
