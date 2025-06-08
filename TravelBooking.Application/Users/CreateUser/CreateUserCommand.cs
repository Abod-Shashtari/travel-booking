using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Users.CreateUser;

public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword
):IRequest<Result<Guid>>;