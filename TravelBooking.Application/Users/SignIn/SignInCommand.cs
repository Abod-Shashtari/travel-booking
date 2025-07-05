using System.ComponentModel.DataAnnotations;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Users.SignIn;

public record SignInCommand(
    string Email,
    string Password
):IRequest<Result<JwtTokenResponse?>>;