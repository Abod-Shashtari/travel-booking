using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Users.SignOut;

public record SignOutAllDevicesCommand(Guid UserId):IRequest<Result>;