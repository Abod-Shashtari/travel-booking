using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Reviews.Entities;

namespace TravelBooking.Infrastructure.Repositories;

[ServiceImplementation]
[RegisterAs<IRepository<Review>>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class ReviewRepository:Repository<Review>
{
    private readonly TravelBookingDbContext _context;
    
    public ReviewRepository(TravelBookingDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<bool> IsExistAsync(Review review, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews.AnyAsync(r=>r.UserId == review.UserId && r.HotelId==review.HotelId, cancellationToken: cancellationToken);
    }
}