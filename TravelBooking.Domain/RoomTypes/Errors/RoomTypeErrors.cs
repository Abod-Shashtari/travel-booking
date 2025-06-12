using TravelBooking.Domain.Common;

namespace TravelBooking.Domain.RoomTypes.Errors;

public class RoomTypeErrors
{
    public static Error RoomTypeAlreadyExists() =>
        new("RoomType.RoomTypeAlreadyExists",
            "This Room Type is already exists in the system",
            ErrorType.Conflict
        );
    public static Error RoomTypeNotFound() =>
        new("RoomType.NotFound",
            "Room Type with the specified ID was not found",
            ErrorType.NotFound
        );
}