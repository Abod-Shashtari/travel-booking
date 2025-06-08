using FluentValidation;

namespace TravelBooking.Application.Users.SignIn;

public class SignInCommandValidator:AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
    {
        RuleFor(x=>x.Email).NotEmpty()
            .EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}