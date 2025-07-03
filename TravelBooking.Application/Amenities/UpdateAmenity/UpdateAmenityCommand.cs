using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Amenities.UpdateAmenity;

public record UpdateAmenityCommand(Guid AmenityId,string Name,string? Description):IRequest<Result>;