using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Infrastructure.Repositories;

namespace Infrastructure.Tests;

public class BookingRepositoryTests : RepositoryTestsBase<Booking>
{
    public BookingRepositoryTests() : base(ctx => new BookingRepository(ctx)) {}

    public override Booking CreateEntity()
    {
        return TestDataFactory.CreateBooking(Context,Fixture); 
    }

    protected override DbSet<Booking> DbSet => Context.Bookings;
}
