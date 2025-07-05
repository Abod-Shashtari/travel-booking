using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelBooking.Domain.Bookings.Entities;

namespace TravelBooking.Infrastructure.Configurations;

public class BookingConfiguration:IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasIndex(b => new { b.CheckIn, b.CheckOut });
        
        builder
            .HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .OnDelete(DeleteBehavior.Restrict);
    }
}