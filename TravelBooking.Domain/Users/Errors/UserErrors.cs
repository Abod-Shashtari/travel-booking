using TravelBooking.Domain.Common;

namespace TravelBooking.Domain.Users.Errors;

public static class UserErrors
{
    public static Error EmailAlreadyUsed(string email) =>
        new("User.EmailAlreadyUsed", 
            $"Email {email} is already used", 
            ErrorType.Conflict
        );
    
    public static Error InvalidCredentialException() =>
        new("User.InvalidCredential", 
            "Invalid Email or Password", 
            ErrorType.Unauthorized
        );
}