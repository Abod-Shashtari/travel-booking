using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Amenities.AddAmenityToRoomType;

public record AddAmenityToRoomTypeCommand(Guid RoomTypeId,Guid AmenityId):IRequest<Result>;