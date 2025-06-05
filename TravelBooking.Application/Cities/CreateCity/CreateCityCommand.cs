using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Cities.CreateCity;

public record CreateCityCommand(string Name,string Country, string PostOffice):IRequest<Result<Guid>>;