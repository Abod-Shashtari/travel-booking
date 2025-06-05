using TravelBooking.Domain.Common;

namespace TravelBooking.Domain.Cities.Errors;

public static class CityErrors
{
    public static Error CityAlreadyExists() =>
        new("City.CityAlreadyExists",
            "This City is already exists in the system",
            ErrorType.Conflict
        );
}