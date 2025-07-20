using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Infrastructure.Repositories;

namespace Infrastructure.Tests;

public class AmenityRepositoryTests : RepositoryTestsBase<Amenity>
{
    public AmenityRepositoryTests() : base(ctx => new AmenityRepository(ctx)) {}

    protected override Amenity CreateEntity()
    {
        return TestDataFactory.CreateAmenity(Fixture);
    }

    protected override DbSet<Amenity> DbSet => Context.Amenities;
}