using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Web.Requests.Hotels;

public record CreateHotelRequest(
    string Name,
    Location Location,
    Guid CityId,
    Guid OwnerId
);