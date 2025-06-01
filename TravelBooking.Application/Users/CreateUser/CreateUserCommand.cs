using System.ComponentModel.DataAnnotations;
using MediatR;

namespace TravelBooking.Application.Users.CreateUser;

public record CreateUserCommand : IRequest<Guid>
{
    [Required]
    public string FirstName{get;init;}=string.Empty;
    [Required]
    public string LastName { get; init; }=string.Empty;
    [Required,EmailAddress]
    public string Email { get; init; }=string.Empty;
    [Required]
    [MinLength(8,ErrorMessage ="Password must be 8 characters or more")]
    public string Password { get; init; }=string.Empty;
    [Compare("Password", ErrorMessage="Passwords do not match.")]
    public string ConfirmPassword { get; init; }=string.Empty;
}
