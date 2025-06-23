using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Bookings.CreateBooking;

public record CreateBookingCommand(
    Guid UserId,
    Guid HotelId,
    List<Guid> Rooms,
    DateTimeOffset CheckIn,
    DateTimeOffset CheckOut
):IRequest<Result<BookingResponse?>>;

