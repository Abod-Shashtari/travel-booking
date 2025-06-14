using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Cities.DeleteCity;

public record DeleteCityCommand(Guid CityId):IRequest<Result>;