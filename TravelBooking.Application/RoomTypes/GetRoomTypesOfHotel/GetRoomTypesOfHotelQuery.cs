using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.RoomTypes.GetRoomTypesOfHotel;

public record GetRoomTypesOfHotelQuery(Guid HotelId, int PageNumber, int PageSize):IRequest<Result<PaginatedList<RoomTypeResponse>?>>;