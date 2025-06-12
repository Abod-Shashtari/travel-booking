
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.RoomTypes.GetRoomType;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.RoomTypes.GetRoomTypes;

public record GetRoomTypesQuery(int PageNumber, int PageSize):IRequest<Result<PaginatedList<RoomTypeResponse>>>;