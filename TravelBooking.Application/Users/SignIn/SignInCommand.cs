using System.ComponentModel.DataAnnotations;
using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Users.SignIn;

public record SignInCommand(
    [Required,EmailAddress]
    string Email,
    [Required]
    string Password
):IRequest<Result<string?>>;