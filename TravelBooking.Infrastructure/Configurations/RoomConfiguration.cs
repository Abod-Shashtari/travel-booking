using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Infrastructure.Configurations;

public class RoomConfiguration:IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasIndex(r=>r.AdultCapacity);
        builder.HasIndex(r=>r.ChildrenCapacity);
        builder.HasIndex(r => new { r.AdultCapacity, r.ChildrenCapacity });
    }
}