namespace TravelBooking.Web.Requests.Bookings;

public record GetBookingsRequest(
    int PageNumber = 1,
    int PageSize = 10
);