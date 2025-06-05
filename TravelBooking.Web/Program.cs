using AttributeBasedRegistration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Application.Common.Profiles;
using TravelBooking.Application.Users.CreateUser;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Infrastructure;
using TravelBooking.Infrastructure.Options;
using TravelBooking.Web.Extensions;
using TravelBooking.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddDbContext<TravelBookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddAutoMapper(typeof(UserProfile));

builder.Services.AddAttributeDefinedServices(typeof(TravelBookingDbContext).Assembly);
builder.Services.AddMediatR(configuration=>
    configuration.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly)
);

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddControllers();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();