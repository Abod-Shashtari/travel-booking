using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Web.Requests.Hotels;

public record UpdateHotelRequest(
    string Name,
    string? Description,
    Location Location,
    Guid CityId,
    Guid OwnerId
);
