using MediatR;

namespace TravelBooking.Application.Users.SignOut;

public record SignOutAllDevicesCommand(Guid UserId):IRequest;