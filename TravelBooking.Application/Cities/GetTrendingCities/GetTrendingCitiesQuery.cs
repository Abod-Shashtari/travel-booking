using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Cities.GetTrendingCities;

public record GetTrendingCitiesQuery():IRequest<Result<PaginatedList<CityResponse>>>;