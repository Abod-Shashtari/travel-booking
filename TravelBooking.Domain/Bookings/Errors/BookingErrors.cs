using TravelBooking.Domain.Common;

namespace TravelBooking.Domain.Bookings.Errors;

public static class BookingErrors
{
    public static Error BookingConflict() =>
        new("Booking.Conflict",
            "Booking conflict. One or more rooms are already booked for the specified dates",
            ErrorType.Conflict
        );
    
    public static Error BookingNotFound() =>
        new("Booking.NotFound",
            "Booking with the specified ID was not found",
            ErrorType.NotFound
        );
    public static Error BookingUncancellable() =>
        new("Booking.Uncancellable",
            "Booking cannot be canceled because it is already confirmed or completed.",
            ErrorType.BadRequest
        );
    
    public static Error BookingCannotBeConfirmed() =>
        new(
            "Booking.CannotBeConfirmed",
            "The booking could not be confirmed.",
            ErrorType.BadRequest
        );
    
    public static Error UserNotOwnerOfBooking() =>
        new("Booking.UserNotOwner",
            "User is not allowed to manage this booking because they are not the owner.",
            ErrorType.Forbidden
        );
}