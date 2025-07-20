using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.UserActivity.Entites;
using TravelBooking.Infrastructure.Repositories;

namespace Infrastructure.Tests;

public class HotelVisitRepositoryTests : RepositoryTestsBase<HotelVisit>
{
    public HotelVisitRepositoryTests() : base(ctx => new HotelVisitRepository(ctx)) {}

    protected override HotelVisit CreateEntity()
    {
        return TestDataFactory.CreateHotelVisit(Context,Fixture);
    }

    protected override DbSet<HotelVisit> DbSet => Context.HotelVisits;
}