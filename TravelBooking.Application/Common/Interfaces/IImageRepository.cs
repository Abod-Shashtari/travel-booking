using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Images.Entities;

namespace TravelBooking.Application.Common.Interfaces;

public interface IImageRepository:IRepository<Image>
{
    Task<ImageUsageInfo?> IsImageUsedAsThumbnailsAsync(Guid imageId);
}