using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Bookings.ConfirmBooking;

public record ConfirmBookingCommand(Guid BookingId, string Email, long Amount):IRequest<Result>;