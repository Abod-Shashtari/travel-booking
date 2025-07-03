using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Infrastructure.Repositories;

namespace Infrastructure.Tests;

public class RoomRepositoryTests : RepositoryTestsBase<Room>
{
    public RoomRepositoryTests() : base(ctx => new RoomRepository(ctx)) {}

    public override Room CreateEntity()
    {
        return TestDataFactory.CreateRoom(Context,Fixture);
    }

    protected override DbSet<Room> DbSet => Context.Rooms;
}