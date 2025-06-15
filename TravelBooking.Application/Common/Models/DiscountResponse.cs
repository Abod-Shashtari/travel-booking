namespace TravelBooking.Application.Common.Models;

public record DiscountResponse(Guid Id,Guid RoomTypeId, decimal Percentage, DateTime StartDate, DateTime EndDate);