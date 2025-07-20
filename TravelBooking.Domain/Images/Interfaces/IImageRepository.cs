using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Images.Entities;

namespace TravelBooking.Domain.Images.Interfaces;

public interface IImageRepository:IRepository<Image>
{
    Task<ImageUsageInfo?> IsImageUsedAsThumbnailsAsync(Guid imageId);
}