using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Rooms.GetRooms;

public record GetRoomsQuery(int PageNumber = 1, int PageSize = 10):IRequest<Result<PaginatedList<RoomResponse>>>;