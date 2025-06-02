using System.ComponentModel.DataAnnotations;
using MediatR;

namespace TravelBooking.Application.Users.SignIn;

public record SignInCommand(
    [Required,EmailAddress]
    string Email,
    [Required]
    string Password
):IRequest<string>;