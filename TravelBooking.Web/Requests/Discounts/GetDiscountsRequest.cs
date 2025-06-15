namespace TravelBooking.Web.Requests.Discounts;

public record GetDiscountsRequest(
    int PageNumber = 1,
    int PageSize = 10
);