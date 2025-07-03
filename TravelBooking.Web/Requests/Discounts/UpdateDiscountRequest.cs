namespace TravelBooking.Web.Requests.Discounts;

public record UpdateDiscountRequest(Guid RoomTypeId, decimal Percentage, DateTime StartDate, DateTime EndDate);
