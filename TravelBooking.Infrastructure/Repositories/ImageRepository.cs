using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Images.Entities;

namespace TravelBooking.Infrastructure.Repositories;

[ServiceImplementation]
[RegisterAs<IRepository<Image>>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class ImageRepository:Repository<Image>
{
    private readonly TravelBookingDbContext _context;
    
    public ImageRepository(TravelBookingDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<bool> IsExistAsync(Image image, CancellationToken cancellationToken = default)
    {
        return await _context.Images.AnyAsync(i=>i.Url == image.Url && i.EntityId == image.EntityId, cancellationToken);
    }
}