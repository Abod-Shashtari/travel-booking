using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Cities.GetCities;

public record GetCitiesQuery(int PageNumber = 1, int PageSize = 10):IRequest<Result<PaginatedList<CityResponse>>>;