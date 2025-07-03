namespace TravelBooking.Web.Requests.Bookings;

public record CreateBookingRequest(
    Guid HotelId,
    List<Guid> Rooms,
    DateTimeOffset CheckIn,
    DateTimeOffset CheckOut
);