using AutoMapper;
using TravelBooking.Application.Users.SignIn;
using TravelBooking.Web.Requests.Users;

namespace TravelBooking.Web.Profiles;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<SignInRequest, SignInCommand>();
    }
}