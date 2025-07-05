using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelBooking.Domain.Cities.Entities;

namespace TravelBooking.Infrastructure.Configurations;

public class CityConfiguration:IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.HasIndex(c=>c.Name);
        
        builder
            .HasOne(h=>h.ThumbnailImage)
            .WithMany()
            .HasForeignKey(p => p.ThumbnailImageId)
            .IsRequired(false);
    }
}