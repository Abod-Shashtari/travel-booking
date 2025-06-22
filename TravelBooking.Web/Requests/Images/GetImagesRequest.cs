namespace TravelBooking.Web.Requests.Images;

public record GetImagesRequest(
    int PageNumber = 1,
    int PageSize = 10
);
