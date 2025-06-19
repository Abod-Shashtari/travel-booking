using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Images.DeleteImage;

public record DeleteImageCommand(Guid ImageId):IRequest<Result>;