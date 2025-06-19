using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Infrastructure.Options;

namespace TravelBooking.Infrastructure.Services;

[ServiceImplementation]
[RegisterAs<IImageService>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class ImageService:IImageService
{
    private readonly Cloudinary _cloudinary;

    public ImageService(IOptions<CloudinaryOptions> options)
    {
        var account = new Account(options.Value.CloudName, options.Value.ApiKey, options.Value.ApiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> AddImageAsync(IFormFile file,CancellationToken cancellationToken=default)
    {
        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName,stream),
            UniqueFilename = true,
        };
        var uploadResult = await _cloudinary.UploadAsync(uploadParams,cancellationToken);
        return uploadResult.Url.ToString();
    }

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        var publicId= GetPublicId(imageUrl);
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        return result.Result == "ok";
    }

    private string GetPublicId(string imageUrl)
    {
        var uri = new Uri(imageUrl);
        var segments = uri.AbsolutePath.Split('/');
        var uploadIndex = Array.IndexOf(segments, "upload");
    
        if (uploadIndex == -1 || uploadIndex >= segments.Length - 1)
            return string.Empty;
    
        var remainingSegments = segments.Skip(uploadIndex + 1).ToArray();
    
        var startIndex = 0;
        if (remainingSegments.Length > 0 && 
            remainingSegments[0].StartsWith("v") && 
            remainingSegments[0].Length > 1 && 
            remainingSegments[0].Substring(1).All(char.IsDigit))
        {
            startIndex = 1;
        }
    
        var publicIdWithExtension = string.Join("/", remainingSegments.Skip(startIndex));
    
        var dotIndex = publicIdWithExtension.LastIndexOf('.');
        var publicId = dotIndex > 0
            ? publicIdWithExtension.Substring(0, dotIndex)
            : publicIdWithExtension;
    
        return publicId;
    }
}