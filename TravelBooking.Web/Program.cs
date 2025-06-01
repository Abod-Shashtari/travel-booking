using AttributeBasedRegistration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Application.Users.CreateUser;
using TravelBooking.Domain.Entities;
using TravelBooking.Domain.Interfaces;
using TravelBooking.Infrastructure;
using TravelBooking.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TravelBookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddAttributeDefinedServices(typeof(TravelBookingDbContext).Assembly);

builder.Services.AddMediatR(configuration=>
    configuration.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly)
);
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();
app.MapControllers();
app.Run();