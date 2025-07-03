using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Cities.SetThumbnail;

public record SetCityThumbnailCommand(Guid CityId, Guid ImageId):IRequest<Result>;