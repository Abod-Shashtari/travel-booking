
namespace TravelBooking.Web.Requests.RoomTypes;

public record CreateRoomTypeRequest(Guid HotelId, string Name, string? Description, decimal PricePerNight);
