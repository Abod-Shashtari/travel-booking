
namespace TravelBooking.Application.Common.Models;

public record CityResponse(
    Guid Id,
    string Name,
    string Country,
    string PostOffice,
    int NumberOfHotels
);