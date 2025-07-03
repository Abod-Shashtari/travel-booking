using TravelBooking.Domain.Common;

namespace TravelBooking.Domain.Rooms.Errors;

public class RoomErrors
{
    public static Error RoomAlreadyExists() =>
        new("Room.RoomAlreadyExists",
            "This Room is already exists in the system",
            ErrorType.Conflict
        );
    public static Error RoomDoesNotExists() =>
        new("Room.RoomDoesNotExists",
            "This Room does not exists in the system",
            ErrorType.NotFound
        );
    
    public static Error RoomNotFound() =>
        new("Room.NotFound",
            "Room with the specified ID was not found",
            ErrorType.NotFound
        );
}