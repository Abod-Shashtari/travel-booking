using System.ComponentModel.DataAnnotations;
using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Users.SignIn;

public record SignInCommand(
    string Email,
    string Password
):IRequest<Result<string?>>;