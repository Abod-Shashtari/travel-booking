namespace TravelBooking.Application.Hotels.GetHotels;

public record HotelFilter(
    string? Keyword,
    DateTime? CheckIn,
    DateTime? CheckOut,
    int? NumberOfRooms,
    int? NumberOfAdults,
    int? NumberOfChildren,
    decimal? MinPrice,
    decimal? MaxPrice,
    double? StarRating,
    List<string>? RoomTypes,
    List<string>? Amenities
);