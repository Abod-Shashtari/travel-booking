using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Reviews.GetReviews;

public record GetReviewsQuery(Guid HotelId,int PageNumber, int PageSize):IRequest<Result<PaginatedList<ReviewResponse>?>>;
