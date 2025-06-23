using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Bookings.CancelBooking;

public record CancelBookingCommand(Guid UserId, Guid BookingId):IRequest<Result>;