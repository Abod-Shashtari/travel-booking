
namespace TravelBooking.Web.Requests.Rooms;

public record CreateRoomRequest(string Number,Guid RoomTypeId,int AdultCapacity,int ChildrenCapacity);