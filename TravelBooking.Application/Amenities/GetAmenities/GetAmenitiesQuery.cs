using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Amenities.GetAmenities;

public record GetAmenitiesQuery(int PageNumber = 1, int PageSize = 10):IRequest<Result<PaginatedList<AmenityResponse>>>;