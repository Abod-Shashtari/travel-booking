using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using TravelBooking.Domain.Common;

namespace TravelBooking.Web.Extensions;

public static class ControllerExtensions
{
    public static IActionResult HandleFailure(this ControllerBase controller ,Error error)
    {
        
        var (statusCode, title, problemType) = error.Type switch 
        { 
            ErrorType.NotFound => (
                StatusCodes.Status404NotFound, 
                "Resource Not Found", 
                "https://tools.ietf.org/html/rfc9110#section-15.5.5"
            ), 
            ErrorType.Conflict => (
                StatusCodes.Status409Conflict, 
                "Conflict", 
                "https://tools.ietf.org/html/rfc9110#section-15.5.10"
            ), 
            ErrorType.Unauthorized => (
                StatusCodes.Status401Unauthorized, 
                "Unauthorized", 
                "https://tools.ietf.org/html/rfc9110#section-15.5.2"
            ), 
            _ => (
                StatusCodes.Status400BadRequest, 
                "Bad Request", 
                "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            ) 
        }; 
        
        var problemDetails = new ProblemDetails{
            Type = problemType,
            Title = title,
            Status = statusCode,
            Detail = error.Message,
            Instance = controller.HttpContext.Request.Path,
            Extensions =
            {
                ["timestamp"] = DateTime.UtcNow,
                ["errorCode"] = error.Code
            }
        };
        return new ObjectResult(problemDetails);
    }
}