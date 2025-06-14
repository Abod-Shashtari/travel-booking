using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Cities.GetCity;

public record GetCityQuery(Guid CityId):IRequest<Result<CityResponse?>>;