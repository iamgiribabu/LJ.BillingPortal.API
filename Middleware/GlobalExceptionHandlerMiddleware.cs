using System.Net;
using System.Text.Json;
using LJ.BillingPortal.API.Exceptions;

namespace LJ.BillingPortal.API.Middleware;

/// <summary>
/// Global exception handling middleware for standardized error responses
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Message = exception.Message,
            Timestamp = DateTime.UtcNow
        };

        switch (exception)
        {
            case ValidationException vex:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Validation failed";
                response.Errors = vex.Failures;
                break;

            case NotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                break;

            case BusinessLogicException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;

            case ArgumentNullException:
            case ArgumentException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Invalid argument provided";
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An internal server error occurred";
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}

/// <summary>
/// Standardized error response model
/// </summary>
public class ErrorResponse
{
    public string Message { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public IDictionary<string, string[]>? Errors { get; set; }
}
