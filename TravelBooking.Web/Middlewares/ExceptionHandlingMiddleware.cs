using Microsoft.AspNetCore.Mvc;

namespace TravelBooking.Web.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }
    private async Task HandleExceptionAsync(
        HttpContext httpContext,
        Exception exception)
    {
        var statusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Type = exception.GetType().Name,
            Title = "An error occurred",
            Status = statusCode,
            Detail = exception.Message
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails);
    }
}