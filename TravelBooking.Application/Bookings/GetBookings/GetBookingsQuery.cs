using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Bookings.GetBookings;

public record GetBookingsQuery(Guid UserId ,int PageNumber = 1, int PageSize = 10):IRequest<Result<PaginatedList<BookingResponse>>>;