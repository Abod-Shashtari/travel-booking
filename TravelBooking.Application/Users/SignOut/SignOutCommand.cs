using MediatR;

namespace TravelBooking.Application.Users.SignOut;

public record SignOutCommand(string TokenJti):IRequest;
