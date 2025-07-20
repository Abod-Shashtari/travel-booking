using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Images.Errors;
using TravelBooking.Domain.Images.Interfaces;

namespace TravelBooking.Application.Images.UploadImage;

public class UploadImageCommandHandler:IRequestHandler<UploadImageCommand, Result<ImageFullResponse?>>
{
    private readonly IImageService _imageService;
    private readonly IImageRepository _imageRepository;
    private readonly IMapper _mapper;

    public UploadImageCommandHandler(IImageService imageService, IImageRepository imageRepository, IMapper mapper)
    {
        _imageService = imageService;
        _imageRepository = imageRepository;
        _mapper = mapper;
    }

    public async Task<Result<ImageFullResponse?>> Handle(UploadImageCommand request, CancellationToken cancellationToken)
    {
        var requestEntityName=request.EntityName.Replace("-","");
        if (!Enum.TryParse<EntityType>(requestEntityName, ignoreCase: true, out var entityType))
            return Result<ImageFullResponse?>.Failure(ImageErrors.ImageInvalidEntityType());
        
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
            return Result<ImageFullResponse?>.Failure(ImageErrors.ImageAlreadyExists());
        }
        
        await _imageRepository.AddAsync(image, cancellationToken);
        await _imageRepository.SaveChangesAsync(cancellationToken);
        var imageFullResponse=_mapper.Map<ImageFullResponse>(image);
        return Result<ImageFullResponse?>.Success(imageFullResponse);
    }
}