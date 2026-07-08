using BookstoreApi.Application.Common;
using BookstoreApi.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace BookstoreApi.API.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException ex => (StatusCodes.Status404NotFound, ex.Message),
            ConflictException ex => (StatusCodes.Status409Conflict, ex.Message),
            BusinessRuleException ex => (StatusCodes.Status422UnprocessableEntity, ex.Message),
            UnauthorizedAccessException ex => (StatusCodes.Status401Unauthorized, ex.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        var response = ApiResponse<object>.FailResult(message);
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
