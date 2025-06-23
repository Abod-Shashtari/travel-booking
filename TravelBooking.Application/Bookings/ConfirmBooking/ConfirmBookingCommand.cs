using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Bookings.ConfirmBooking;

public record ConfirmBookingCommand(Guid UserId,Guid BookingId):IRequest<Result>;