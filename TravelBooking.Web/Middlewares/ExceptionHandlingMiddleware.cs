using Microsoft.AspNetCore.Mvc;

namespace TravelBooking.Web.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception ex)
        {
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