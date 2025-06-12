using AutoMapper;
using TravelBooking.Application.Users.CreateUser;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Application.Common.Profiles;

public class UserProfile:Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserCommand, User>();
    }
}