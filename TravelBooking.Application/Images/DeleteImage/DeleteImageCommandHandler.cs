using MediatR;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Images.Errors;
using TravelBooking.Domain.Images.Interfaces;

namespace TravelBooking.Application.Images.DeleteImage;

public class DeleteImageCommandHandler:IRequestHandler<DeleteImageCommand, Result>
{
    private readonly IImageService _imageService;
    private readonly IImageRepository _imageRepository;

    public DeleteImageCommandHandler(IImageService imageService, IImageRepository imageRepository)
    {
        _imageService = imageService;
        _imageRepository = imageRepository;
    }

    public async Task<Result> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
    {
        var image = await _imageRepository.GetByIdAsync(request.ImageId,cancellationToken);
        if(image == null) return Result.Failure(ImageErrors.ImageNotFound());

        var imageUsageInfo = await _imageRepository.IsImageUsedAsThumbnailsAsync(request.ImageId);
        if(imageUsageInfo!=null) 
            return Result.Failure(ImageErrors.ImageInUse(imageUsageInfo.EntityType, imageUsageInfo.EntityId));
        
        var result = await _imageService.DeleteImageAsync(image.Url);
        if(!result) return Result.Failure(ImageErrors.ErrorWhileDeleting()); 
        _imageRepository.Delete(image);
        await _imageRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}