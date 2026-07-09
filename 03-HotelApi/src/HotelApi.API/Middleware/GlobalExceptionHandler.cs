using HotelApi.Application.Common;
using HotelApi.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace HotelApi.API.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex, CancellationToken ct)
    {
        logger.LogError(ex, "Unhandled exception: {Msg}", ex.Message);

        var (code, msg) = ex switch
        {
            NotFoundException      => (HttpStatusCode.NotFound,              ex.Message),
            ConflictException      => (HttpStatusCode.Conflict,              ex.Message),
            BusinessRuleException  => (HttpStatusCode.UnprocessableEntity,   ex.Message),
            _                      => (HttpStatusCode.InternalServerError,   "An unexpected error occurred.")
        };

        ctx.Response.StatusCode  = (int)code;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsJsonAsync(ApiResponse<object>.FailResult(msg), ct);
        return true;
    }
}
