
namespace TravelBooking.Web.Requests.Rooms;

public record CreateRoomRequest(string Number,int AdultCapacity,int ChildrenCapacity);