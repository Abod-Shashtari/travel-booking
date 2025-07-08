using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Domain.Common;

namespace TravelBooking.Web.Extensions;

public static class ControllerExtensions
{
    public static Guid GetUserId(this ControllerBase controller)
    {
        var userIdClaim = controller.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        return new Guid(userIdClaim!.Value);
    }
    public static Guid? GetUserIdOrNull(this ControllerBase controller)
    {
        var userIdClaim = controller.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return null;
        return new Guid(userIdClaim!.Value);
    }

    public static IActionResult HandleFailure(this ControllerBase controller ,Error error)
    {
        if (error is ValidationError validationResult)
        {
            return new ObjectResult(
                CreateProblemDetails(
                    StatusCodes.Status400BadRequest,
                    error,
                    validationResult.Errors
                )
            );
        }
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
    
    private static ProblemDetails CreateProblemDetails(int status, Error error, Error[] errors) => new() 
        {
            Title = "Validation Error",
            Type = error.Code,
            Detail = error.Message,
            Status = status,
            Extensions = { { nameof(errors), errors } }
        };
}