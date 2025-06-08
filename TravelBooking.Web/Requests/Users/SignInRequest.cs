namespace TravelBooking.Web.Requests.Users;

public record SignInRequest(
    string Email,
    string Password
);