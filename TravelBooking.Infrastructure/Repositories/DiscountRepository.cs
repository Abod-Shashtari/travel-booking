using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Discounts.Entities;

namespace TravelBooking.Infrastructure.Repositories;

[ServiceImplementation]
[RegisterAs<IRepository<Discount>>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class DiscountRepository:Repository<Discount>
{
    private readonly TravelBookingDbContext _context;
    
    public DiscountRepository(TravelBookingDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<bool> IsExistAsync(Discount discount, CancellationToken cancellationToken = default)
    {
        return await _context.Discounts.AnyAsync(
            d=>d.RoomTypeId == discount.RoomTypeId &&
               d.Percentage == discount.Percentage &&
               d.StartDate == discount.StartDate &&
               d.EndDate == discount.EndDate
            ,cancellationToken
        );
    }
}