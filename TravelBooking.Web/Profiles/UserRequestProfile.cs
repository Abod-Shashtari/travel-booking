using AutoMapper;
using TravelBooking.Application.Users.CreateUser;
using TravelBooking.Application.Users.SignIn;
using TravelBooking.Web.Requests.Users;

namespace TravelBooking.Web.Profiles;

public class UserRequestProfile: Profile
{
    public UserRequestProfile()
    {
        CreateMap<SignInRequest, SignInCommand>();
        CreateMap<CreateUserRequest, CreateUserCommand>();
    }
}