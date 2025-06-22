using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Common.Models;

public record HotelResponse(
    Guid Id,
    string Name,
    Location Location,
    string? Description=null,
    string CityName="",
    Guid CityId=default,
    Guid OwnerId=default,
    DateTimeOffset CreatedAt=default,
    DateTimeOffset? ModifiedAt=null,
    ImageResponse? Thumbnail=null
);