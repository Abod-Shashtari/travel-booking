using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Infrastructure.Repositories;

namespace Infrastructure.Tests;

public class HotelRepositoryTests:RepositoryTestsBase<Hotel>{
    
    public HotelRepositoryTests() : base(ctx => new HotelRepository(ctx)) {}
    public override Hotel CreateEntity()
    {
        var city = TestDataFactory.CreateCity(Fixture); 
        Context.Cities.Add(city);
        Context.SaveChanges();
        
        var user = TestDataFactory.CreateUser(Fixture); 
        Context.Users.Add(user);
        Context.SaveChanges();
        
        return TestDataFactory.CreateHotel(Context,Fixture); 
    }

    protected override DbSet<Hotel> DbSet => Context.Hotels;
}