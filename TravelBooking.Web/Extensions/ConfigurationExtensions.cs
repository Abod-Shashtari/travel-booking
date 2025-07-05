using Stripe;
using TravelBooking.Infrastructure.Options;

namespace TravelBooking.Web.Extensions;

public static class ConfigurationExtensions
{
    public static void AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.Configure<CloudinaryOptions>(configuration.GetSection("Cloudinary"));
        services.Configure<StripeOptions>(configuration.GetSection("Stripe"));
        services.Configure<SmtpOptions>(configuration.GetSection("Smtp"));
        
        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
    }
}