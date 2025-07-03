using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Users.SignIn;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    JwtTokenResult GenerateToken(User user);
}