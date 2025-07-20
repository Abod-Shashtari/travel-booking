using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Infrastructure.Repositories;

namespace Infrastructure.Tests;

public class RoomTypeRepositoryTests : RepositoryTestsBase<RoomType>
{
    public RoomTypeRepositoryTests() : base(ctx => new RoomTypeRepository(ctx)) {}

    protected override RoomType CreateEntity()
    {
        return TestDataFactory.CreateRoomType(Context,Fixture);
    }

    protected override DbSet<RoomType> DbSet => Context.RoomsTypes;

}