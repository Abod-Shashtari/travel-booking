using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Amenities.RemoveAmenityFromRoomType;

public record RemoveAmenityFromRoomTypeCommand(Guid RoomTypeId, Guid AmenityId):IRequest<Result>;