using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Amenities.DeleteAmenity;
public record DeleteAmenityCommand(Guid AmenityId):IRequest<Result>;