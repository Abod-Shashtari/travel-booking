namespace TravelBooking.Domain.Exceptions;

public class EmailAlreadyUsed : Exception
{
    public EmailAlreadyUsed(string email) : base($"Email {email} is already used") { }
}