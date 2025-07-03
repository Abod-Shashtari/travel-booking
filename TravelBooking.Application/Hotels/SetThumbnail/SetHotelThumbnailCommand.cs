using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Hotels.SetThumbnail;

public record SetHotelThumbnailCommand(Guid HotelId, Guid ImageId):IRequest<Result>;