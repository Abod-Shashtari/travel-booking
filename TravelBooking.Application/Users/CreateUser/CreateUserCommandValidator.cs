using FluentValidation;

namespace TravelBooking.Application.Users.CreateUser;

public class CreateUserCommandValidator:AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
            .Matches("[A-Z]").WithMessage("At least one uppercase letter is required.")
            .Matches("[a-z]").WithMessage("At least one lowercase letter is required.")
            .Matches("[0-9]").WithMessage("At least one number is required.");
        RuleFor(x => x.ConfirmPassword).NotEmpty()
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match.");
    }
}