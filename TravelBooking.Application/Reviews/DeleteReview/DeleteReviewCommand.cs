using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Reviews.DeleteReview;

public record DeleteReviewCommand(Guid UserId, Guid ReviewId):IRequest<Result>;