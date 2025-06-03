using Microsoft.AspNetCore.Mvc;
using TravelBooking.Domain.Common;

namespace TravelBooking.Web.Extensions;

public static class ControllerExtensions
{
    public static IActionResult HandleFailure(this ControllerBase controller ,Error error)
    {
        var problemDetails = error.Type switch
        {
            ErrorType.NotFound => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Resource Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = error.Message,
                Instance = controller.HttpContext.Request.Path
            },
            ErrorType.Conflict => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                Title = "Conflict",
                Status = StatusCodes.Status409Conflict,
                Detail = error.Message,
                Instance = controller.HttpContext.Request.Path
            },
            ErrorType.Unauthorized => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = error.Message,
                Instance = controller.HttpContext.Request.Path
            },
            _ => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = error.Message,
                Instance = controller.HttpContext.Request.Path
            }
        };

        // Add error code to extensions
        problemDetails.Extensions["errorCode"] = error.Code;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
    }
}