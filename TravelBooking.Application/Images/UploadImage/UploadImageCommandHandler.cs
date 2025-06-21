using MediatR;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Images.Errors;

namespace TravelBooking.Application.Images.UploadImage;

public class UploadImageCommandHandler:IRequestHandler<UploadImageCommand, Result>
{
    private readonly IImageService _imageService;
    private readonly IRepository<Image> _imageRepository;

    public UploadImageCommandHandler(IImageService imageService, IRepository<Image> imageRepository)
    {
        _imageService = imageService;
        _imageRepository = imageRepository;
    }

    public async Task<Result> Handle(UploadImageCommand request, CancellationToken cancellationToken)
    {
        var entityType= Enum.GetValues<EntityType>()
            .FirstOrDefault(e => string.Equals(e.ToString(), request.EntityName, StringComparison.OrdinalIgnoreCase));
        if(!Enum.IsDefined(typeof(EntityType), request.EntityName))
            return Result.Failure(ImageErrors.ImageInvalidEntityType());
        
        var url = await _imageService.AddImageAsync(request.Image,cancellationToken);
        var image = new Image
        {
            EntityId = request.EntityId,
            Url = url,
            EntityType = entityType
        };

        if (await _imageRepository.IsExistAsync(image, cancellationToken))
        {
            await _imageService.DeleteImageAsync(url);
            return Result.Failure(ImageErrors.ImageAlreadyExists());
        }
        
        await _imageRepository.AddAsync(image, cancellationToken);
        await _imageRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}