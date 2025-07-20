using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Infrastructure.Repositories;

namespace Infrastructure.Tests;

public class ReviewRepositoryTests : RepositoryTestsBase<Review>
{
    public ReviewRepositoryTests() : base(ctx => new ReviewRepository(ctx)) {}

    protected override Review CreateEntity()
    {
        return TestDataFactory.CreateReview(Context,Fixture);
    }

    protected override DbSet<Review> DbSet => Context.Reviews;
}