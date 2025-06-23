using TravelBooking.Domain.Bookings.Entities;

namespace TravelBooking.Application.Common.Models;

public record BookingResponse(
    Guid Id,
    Guid UserId,
    Guid HotelId,
    List<Guid> RoomIds,
    decimal TotalCost,
    DateTimeOffset CheckIn,
    DateTimeOffset CheckOut,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ModifiedAt
);