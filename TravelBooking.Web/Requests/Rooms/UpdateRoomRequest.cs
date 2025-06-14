namespace TravelBooking.Web.Requests.Rooms;

public record UpdateRoomRequest(
    string Number,
    Guid RoomTypeId,
    int AdultCapacity,
    int ChildrenCapacity
);