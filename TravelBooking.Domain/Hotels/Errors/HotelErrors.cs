using TravelBooking.Domain.Common;

namespace TravelBooking.Domain.Hotels.Errors;

public static class HotelErrors
{
    public static Error HotelAlreadyExists() =>
        new("Hotel.HotelAlreadyExists",
            "This Hotel is already exists in the system",
            ErrorType.Conflict
        );
    public static Error HotelNotFound() =>
        new("Hotel.NotFound",
            "Hotel with the specified ID was not found",
            ErrorType.NotFound
        );
}