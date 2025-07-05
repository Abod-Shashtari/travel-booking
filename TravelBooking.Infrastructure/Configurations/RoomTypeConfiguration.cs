using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelBooking.Domain.RoomTypes.Entities;

namespace TravelBooking.Infrastructure.Configurations;

public class RoomTypeConfiguration:IEntityTypeConfiguration<RoomType>
{
    public void Configure(EntityTypeBuilder<RoomType> builder)
    {
        builder.HasIndex(r=>r.PricePerNight);
    }
}