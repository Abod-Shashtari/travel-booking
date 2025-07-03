using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Infrastructure.Repositories;

namespace Infrastructure.Tests;

public class CityRepositoryTests : RepositoryTestsBase<City>
{
    public CityRepositoryTests() : base(ctx => new CityRepository(ctx)) {}

    public override City CreateEntity()
    {
        return TestDataFactory.CreateCity(Fixture);
    }

    protected override DbSet<City> DbSet => Context.Cities;

}