namespace TravelBooking.Application.Common.Models;

public record AmenityResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ModifiedAt
);