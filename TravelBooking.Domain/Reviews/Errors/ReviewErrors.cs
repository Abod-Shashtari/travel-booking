using TravelBooking.Domain.Common;

namespace TravelBooking.Domain.Reviews.Errors;

public class ReviewErrors
{
    public static Error ReviewAlreadyExists() =>
        new("Review.ReviewAlreadyExists",
            "This User has already wrote a review on this Hotel, Instead you can edit your review",
            ErrorType.Conflict
        );
    public static Error ReviewNotFound() =>
        new("Review.NotFound",
            "Review with the specified ID was not found",
            ErrorType.NotFound
        );
    public static Error NotAllowedToUpdateThisReview() =>
        new("Review.NotAllowedToUpdate",
            "This User is not allowed to update this review",
            ErrorType.Forbidden
        );
}