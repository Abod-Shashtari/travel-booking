using AttributeBasedRegistration;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using TravelBooking.Application.Common.Behaviors;
using TravelBooking.Application.Common.Profiles;
using TravelBooking.Application.Users.CreateUser;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Infrastructure;
using TravelBooking.Web.Profiles;

namespace TravelBooking.Web.Extensions;

public static class ApplicationServicesExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(UserProfile));
        services.AddAutoMapper(typeof(UserRequestProfile));

        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly));

        services.AddValidatorsFromAssembly(typeof(CreateUserCommandValidator).Assembly);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddAttributeDefinedServices(typeof(TravelBookingDbContext).Assembly);
        
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
    }
}