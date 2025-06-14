namespace TravelBooking.Web.Requests.Cities;

public record GetCitiesRequest(
    int PageNumber = 1,
    int PageSize = 10
);