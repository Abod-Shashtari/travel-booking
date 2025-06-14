using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Rooms.GetRoom;

public record GetRoomQuery(Guid RoomId):IRequest<Result<RoomResponse?>>;