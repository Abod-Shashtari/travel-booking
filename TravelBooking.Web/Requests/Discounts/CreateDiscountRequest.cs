namespace TravelBooking.Web.Requests.Discounts;

public record CreateDiscountRequest(Guid RoomTypeId, decimal Percentage, DateTime StartDate, DateTime EndDate);
