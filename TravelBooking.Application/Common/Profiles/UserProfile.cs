using AutoMapper;
using TravelBooking.Application.Users.CreateUser;
using TravelBooking.Application.Users.SignIn;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Application.Common.Profiles;

public class UserProfile:Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserCommand, User>();
        CreateMap<CreateUserRequest, CreateUserCommand>();
    }
}