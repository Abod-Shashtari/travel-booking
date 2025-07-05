using Microsoft.EntityFrameworkCore;
using TravelBooking.Infrastructure;

namespace TravelBooking.Web.Extensions;

public static class DatabaseExtensions
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TravelBookingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        );
    }

    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TravelBookingDbContext>();
        context.Database.Migrate();
    }
}