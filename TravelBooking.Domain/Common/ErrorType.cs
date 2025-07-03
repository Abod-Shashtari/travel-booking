namespace TravelBooking.Domain.Common;

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    BadRequest
}