namespace TravelBooking.Web.Requests.Hotels;

public record GetHotelsRequest
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
};