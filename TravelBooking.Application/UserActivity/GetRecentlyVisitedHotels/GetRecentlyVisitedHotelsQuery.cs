using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.UserActivity.GetRecentlyVisitedHotels;

public record GetRecentlyVisitedHotelsQuery(Guid UserId):IRequest<Result<PaginatedList<HotelResponse>>>;