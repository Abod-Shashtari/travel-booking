namespace TravelBooking.Web.Requests.Rooms;

public record GetRoomsRequest(
    int PageNumber = 1,
    int PageSize = 10
);