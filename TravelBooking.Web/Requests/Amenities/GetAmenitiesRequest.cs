namespace TravelBooking.Web.Requests.Amenities;

public record GetAmenitiesRequest(
    int PageNumber = 1,
    int PageSize = 10
);
