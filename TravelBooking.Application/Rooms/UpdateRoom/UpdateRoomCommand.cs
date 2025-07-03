using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Rooms.UpdateRoom;

public record UpdateRoomCommand(
    Guid RoomId,
    string Number,
    Guid RoomTypeId,
    int AdultCapacity,
    int ChildrenCapacity
) : IRequest<Result>;