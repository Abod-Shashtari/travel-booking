using TravelBooking.Domain.Common;

namespace TravelBooking.Domain.Amenities.Errors;

public class AmenityErrors
{
    public static Error AmenityAlreadyExists() =>
        new("Amenity.AmenityAlreadyExists",
            "This Amenity is already exists in the system",
            ErrorType.Conflict
        );
    
    public static Error AmenityNotFound() =>
        new("Amenity.NotFound",
            "Amenity with the specified ID was not found",
            ErrorType.NotFound
        );
}