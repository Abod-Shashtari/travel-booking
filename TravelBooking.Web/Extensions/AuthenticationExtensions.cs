using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TravelBooking.Domain.Authentication.Interfaces;

namespace TravelBooking.Web.Extensions;

public static class AuthenticationExtensions
{
    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication("Bearer").AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = configuration["Jwt:issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:key"]!))
            };
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    var tokenWhiteListRepository = context.HttpContext.RequestServices.GetRequiredService<ITokenWhiteListRepository>();
                    
                    var jtiClaim = context.Principal?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
                    if (jtiClaim==null || !await tokenWhiteListRepository.IsTokenActiveAsync(jtiClaim.Value))
                    {
                        context.Fail("Token is not active or not whitelisted.");
                    }
                }
            };
        });
    }
}