using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Rooms.CreateRoom;

public record CreateRoomCommand(string Number,Guid RoomTypeId,int AdultCapacity,int ChildrenCapacity):IRequest<Result<RoomResponse?>>;