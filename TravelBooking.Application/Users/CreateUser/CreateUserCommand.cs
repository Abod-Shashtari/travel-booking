using System.ComponentModel.DataAnnotations;
using MediatR;

namespace TravelBooking.Application.Users.CreateUser;

public record CreateUserCommand(
    [Required,EmailAddress]
    string Email,
    [Required]
    string Password
):IRequest<Guid>;