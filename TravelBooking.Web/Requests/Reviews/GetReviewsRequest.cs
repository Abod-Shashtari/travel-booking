namespace TravelBooking.Web.Requests.Reviews;

public record GetReviewsRequest(
    int PageNumber = 1,
    int PageSize = 10
);
