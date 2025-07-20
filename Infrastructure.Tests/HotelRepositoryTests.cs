using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Infrastructure.Repositories;

namespace Infrastructure.Tests;

public class HotelRepositoryTests:RepositoryTestsBase<Hotel>{
    
    public HotelRepositoryTests() : base(ctx => new HotelRepository(ctx)) {}

    protected override Hotel CreateEntity()
    {
        return TestDataFactory.CreateHotel(Context,Fixture); 
    }

    protected override DbSet<Hotel> DbSet => Context.Hotels;
}