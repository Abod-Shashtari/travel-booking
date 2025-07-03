namespace TravelBooking.Web.Requests.Users;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword
);
