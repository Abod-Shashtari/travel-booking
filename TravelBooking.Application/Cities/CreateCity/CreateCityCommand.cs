using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Cities.CreateCity;

public record CreateCityCommand(string Name,string Country, string PostOffice):IRequest<Result<CityResponse?>>;