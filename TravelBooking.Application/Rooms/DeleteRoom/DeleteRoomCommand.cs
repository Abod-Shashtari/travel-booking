using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Rooms.DeleteRoom;

public record DeleteRoomCommand(Guid RoomId):IRequest<Result>;