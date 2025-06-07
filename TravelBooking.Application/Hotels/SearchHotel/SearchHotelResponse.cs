using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Hotels.SearchHotel;

public record SearchHotelResponse(Guid Id, string Name, Location Location, Guid CityId, string CityName);