using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Bookings.CompleteBooking;

public record CompleteBookingCommand(Guid BookingId):IRequest<Result>;