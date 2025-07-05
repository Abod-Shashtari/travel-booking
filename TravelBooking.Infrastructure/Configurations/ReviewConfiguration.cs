using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelBooking.Domain.Reviews.Entities;

namespace TravelBooking.Infrastructure.Configurations;

public class ReviewConfiguration:IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder
            .HasOne(h => h.User)
            .WithMany(u=>u.Reviews)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}