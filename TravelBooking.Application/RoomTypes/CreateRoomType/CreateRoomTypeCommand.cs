using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.RoomTypes.CreateRoomType;

public record CreateRoomTypeCommand(Guid HotelId, string Name, string? Description, decimal PricePerNight):IRequest<Result<RoomTypeResponse?>>;
