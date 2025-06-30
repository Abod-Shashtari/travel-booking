using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Infrastructure.Repositories;

namespace Infrastructure.Tests;

public class UserRepositoryTests : RepositoryTestsBase<User>
{
    public UserRepositoryTests() : base(ctx => new UserRepository(ctx)) {}

    public override User CreateEntity()
    {
        return TestDataFactory.CreateUser(Fixture);
    }

    protected override DbSet<User> DbSet => Context.Users;
}