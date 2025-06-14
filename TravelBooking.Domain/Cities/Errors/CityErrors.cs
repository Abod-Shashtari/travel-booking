using TravelBooking.Domain.Common;

namespace TravelBooking.Domain.Cities.Errors;

public static class CityErrors
{
    public static Error CityAlreadyExists() =>
        new("City.CityAlreadyExists",
            "This City is already exists in the system",
            ErrorType.Conflict
        );
    
    public static Error CityNotFound() =>
        new("City.NotFound",
            "City with the specified ID was not found",
            ErrorType.NotFound
        );
}