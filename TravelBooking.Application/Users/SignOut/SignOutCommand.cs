using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Users.SignOut;

public record SignOutCommand(string TokenJti):IRequest<Result>;
