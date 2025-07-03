using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Amenities.GetAmenity;

public record GetAmenityQuery(Guid AmenityId):IRequest<Result<AmenityResponse?>>;