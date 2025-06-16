using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Amenities.CreateAmenity;

public record CreateAmenityCommand(string Name,string? Description):IRequest<Result<AmenityResponse?>>;