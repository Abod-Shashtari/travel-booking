using TravelBooking.Domain.Common;

namespace TravelBooking.Domain.Hotels.Errors;

public static class HotelErrors
{
    public static Error CityAlreadyExists() =>
        new("Hotel.HotelAlreadyExists",
            "This Hotel is already exists in the system",
            ErrorType.Conflict
        );
}