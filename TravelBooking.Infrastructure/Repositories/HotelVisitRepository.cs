using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.UserActivity.Entites;

namespace TravelBooking.Infrastructure.Repositories;

[ServiceImplementation]
[RegisterAs<IRepository<HotelVisit>>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class HotelVisitRepository:Repository<HotelVisit>
{
    private TravelBookingDbContext _context;
    
    public HotelVisitRepository(TravelBookingDbContext context) : base(context)
    {
        _context=context;
    }

    public override async Task<bool> IsExistAsync(HotelVisit hotelVisit, CancellationToken cancellationToken = default)
    {
        return _context.HotelVisits.Any(hv=>hv.HotelId==hotelVisit.HotelId && hv.UserId == hotelVisit.UserId);
    }
}