namespace TravelBooking.Application.Hotels.GetHotels;

public record HotelFilter(
    string? Keyword,
    DateTime? CheckIn,
    DateTime? CheckOut,
    int? NumberOfRooms,
    int? NumberOfAdults,
    int? NumberOfChildren
);