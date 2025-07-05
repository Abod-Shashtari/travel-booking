using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelBooking.Domain.Discounts.Entities;

namespace TravelBooking.Infrastructure.Configurations;

public class DiscountConfiguration:IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.HasIndex(d=>new {d.StartDate,d.EndDate});
    }
}