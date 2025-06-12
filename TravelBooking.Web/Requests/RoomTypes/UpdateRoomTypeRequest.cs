namespace TravelBooking.Web.Requests.RoomTypes;

public record UpdateRoomTypeRequest(Guid HotelId, string Name, string? Description, decimal PricePerNight);