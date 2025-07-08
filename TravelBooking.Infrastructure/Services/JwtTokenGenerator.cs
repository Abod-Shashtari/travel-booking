using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Infrastructure.Options;

namespace TravelBooking.Infrastructure.Services;

[ServiceImplementation]
[RegisterAs<IJwtTokenGenerator>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class JwtTokenGenerator:IJwtTokenGenerator
{
    private readonly JwtOptions _jwtOptions;
    public JwtTokenGenerator(IOptions<JwtOptions> options)
    {
        _jwtOptions = options.Value;
    }

    public JwtTokenResult GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jti = Guid.NewGuid().ToString();
        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Jti, jti),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.UserRole.ToString()),
        ];
        
        var expirationDate = DateTime.UtcNow.AddHours(1);
        var tokenDescription = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expirationDate,
            Issuer = _jwtOptions.Issuer,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)),
                SecurityAlgorithms.HmacSha256Signature
            ),
            NotBefore = DateTime.UtcNow,
        };
        var token = tokenHandler.CreateToken(tokenDescription);
        return new JwtTokenResult(tokenHandler.WriteToken(token) ,jti, expirationDate);
    }
}