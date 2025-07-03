using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Reviews.UpdateReview;

public record UpdateReviewCommand(Guid ReviewId, Guid UserId, string? TextReview, double StarRating):IRequest<Result>;
