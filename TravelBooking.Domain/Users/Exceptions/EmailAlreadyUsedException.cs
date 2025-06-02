namespace TravelBooking.Domain.Users.Exceptions;

public class EmailAlreadyUsedException : Exception
{
    public EmailAlreadyUsedException(string email) : base($"Email {email} is already used") { }
}