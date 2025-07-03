using FluentValidation;

namespace TravelBooking.Application.Reviews.PostReview;

public class PostReviewCommandValidator:AbstractValidator<PostReviewCommand>
{
    public PostReviewCommandValidator()
    {
        RuleFor(x => x.StarRating).NotEmpty()
            .GreaterThanOrEqualTo(0).WithMessage("The Star Rating must be greater than or equal to 0.")
            .LessThanOrEqualTo(5).WithMessage("The Star Rating must be less than or equal to 5.");
        When(x => x.TextReview != null, () =>
        {
            RuleFor(x => x.TextReview)
                .MaximumLength(500)
                .WithMessage("The Text Review must not exceed 500 characters.");
        });
    }
}