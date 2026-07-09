using HotelApi.Application.Common;
using HotelApi.Application.DTOs.Report;
using HotelApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApi.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class ReportsController(ReportService svc) : ControllerBase
{
    [HttpGet("occupancy")]
    public async Task<ActionResult<ApiResponse<OccupancyReportResponse>>> Occupancy()
        => Ok(ApiResponse<OccupancyReportResponse>.SuccessResult(await svc.GetOccupancyAsync(), "Occupancy report generated."));

    [HttpGet("revenue")]
    public async Task<ActionResult<ApiResponse<RevenueReportResponse>>> Revenue(
        [FromQuery] DateTime from, [FromQuery] DateTime to)
        => Ok(ApiResponse<RevenueReportResponse>.SuccessResult(await svc.GetRevenueAsync(from, to), "Revenue report generated."));
}
