using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelBooking.Domain.UserActivity.Entites;

namespace TravelBooking.Infrastructure.Configurations;

public class HotelVisitConfiguration:IEntityTypeConfiguration<HotelVisit>
{
    public void Configure(EntityTypeBuilder<HotelVisit> builder)
    {
        builder
            .HasOne(h => h.User)
            .WithMany(u=>u.HotelVisits)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}