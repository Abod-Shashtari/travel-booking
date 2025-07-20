using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Images.Interfaces;

namespace TravelBooking.Infrastructure.Repositories;

[ServiceImplementation]
[RegisterAs<IImageRepository>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class ImageRepository:Repository<Image>,IImageRepository
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

    public async Task<ImageUsageInfo?> IsImageUsedAsThumbnailsAsync(Guid imageId)
    {
        var hotel =  await _context.Hotels.FirstOrDefaultAsync(h => h.ThumbnailImageId == imageId);
        if (hotel != null) return new ImageUsageInfo("Hotels", hotel.Id);
        
        var city =  await _context.Cities.FirstOrDefaultAsync(c => c.ThumbnailImageId == imageId);
        if (city != null) return new ImageUsageInfo("Cities", city.Id);
        
        return null;
    }
}