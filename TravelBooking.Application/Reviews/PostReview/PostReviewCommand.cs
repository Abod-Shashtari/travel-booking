using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Reviews.PostReview;

public record PostReviewCommand(Guid UserId, Guid HotelId, string? TextReview, double StarRating):IRequest<Result<ReviewResponse?>>;