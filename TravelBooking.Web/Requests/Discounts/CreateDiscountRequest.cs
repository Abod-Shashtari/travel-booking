namespace TravelBooking.Web.Requests.Discounts;

public record CreateDiscountRequest(decimal Percentage, DateTime StartDate, DateTime EndDate);
