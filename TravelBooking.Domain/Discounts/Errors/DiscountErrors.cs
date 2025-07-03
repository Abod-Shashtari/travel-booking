using TravelBooking.Domain.Common;

namespace TravelBooking.Domain.Discounts.Errors;

public class DiscountErrors
{
    public static Error DiscountAlreadyExists() =>
        new("Discount.DiscountAlreadyExists",
            "This Discount is already exists in the system",
            ErrorType.Conflict
        );
    public static Error DiscountNotFound() =>
        new("Discount.NotFound",
            "Discount with the specified ID was not found",
            ErrorType.NotFound
        );
}