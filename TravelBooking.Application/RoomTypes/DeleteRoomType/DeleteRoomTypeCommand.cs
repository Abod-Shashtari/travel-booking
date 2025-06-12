using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.RoomTypes.DeleteRoomType;

public record DeleteRoomTypeCommand(Guid RoomTypeId):IRequest<Result>;