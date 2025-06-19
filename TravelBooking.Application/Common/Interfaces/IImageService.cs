using Microsoft.AspNetCore.Http;

namespace TravelBooking.Application.Common.Interfaces;

public interface IImageService
{
    Task<string> AddImageAsync(IFormFile file,CancellationToken cancellationToken=default);
    Task<bool> DeleteImageAsync(string fileName);
}