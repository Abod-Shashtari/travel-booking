using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Infrastructure.Repositories;

namespace Infrastructure.Tests;

public class DiscountRepositoryTests : RepositoryTestsBase<Discount>
{
    public DiscountRepositoryTests() : base(ctx => new DiscountRepository(ctx)) {}

    protected override Discount CreateEntity()
    {
        return TestDataFactory.CreateDiscount(Context,Fixture);
    }

    protected override DbSet<Discount> DbSet => Context.Discounts;
}