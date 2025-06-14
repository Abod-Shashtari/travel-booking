using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Cities.UpdateCity;

public record UpdateCityCommand(Guid CityId,string Name,string Country, string PostOffice):IRequest<Result>;