using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.RoomTypes.UpdateRoomType;

public record UpdateRoomTypeCommand(
    Guid RoomTypeId,
    Guid HotelId,
    string Name,
    string? Description,
    decimal PricePerNight
) : IRequest<Result>;