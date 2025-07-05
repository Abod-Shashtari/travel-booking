using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Infrastructure.Configurations;

public class HotelConfiguration:IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.HasIndex(h=> h.Name);
        builder.HasIndex(h=> h.StarRating);
        builder.HasIndex(h=> new {h.Name,h.StarRating});
        
        builder.ComplexProperty(h=>h.Location);
        
        builder
            .HasOne(h=>h.ThumbnailImage)
            .WithMany()
            .HasForeignKey(p => p.ThumbnailImageId)
            .IsRequired(false);
        
        builder
            .HasMany(h => h.Bookings)
            .WithOne(b => b.Hotel)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}