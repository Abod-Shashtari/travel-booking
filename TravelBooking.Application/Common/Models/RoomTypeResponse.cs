namespace TravelBooking.Application.Common.Models;

public record RoomTypeResponse(
    Guid Id,
    Guid HotelId,
    string Name,
    string? Description = null,
    decimal PricePerNight = 0,
    decimal? DiscountedPricePerNight = null,
    DateTimeOffset CreatedAt = default,
    DateTimeOffset? ModifiedAt = null
);