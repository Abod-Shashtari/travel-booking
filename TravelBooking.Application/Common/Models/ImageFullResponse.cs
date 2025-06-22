using TravelBooking.Domain.Images.Entities;

namespace TravelBooking.Application.Common.Models;

public record ImageFullResponse(
    Guid Id,
    string Url,
    Guid EntityId,
    EntityType EntityType,
    DateTimeOffset CreatedAt,
    DateTimeOffset ModifiedAt
);