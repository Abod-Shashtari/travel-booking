using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.RoomTypes.GetRoomType;

public record GetRoomTypeQuery(Guid RoomTypeId):IRequest<Result<RoomTypeResponse?>>;