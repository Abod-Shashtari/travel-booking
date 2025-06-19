using MediatR;
using Microsoft.AspNetCore.Http;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Application.Images.UploadImage;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Images.Errors;

namespace TravelBooking.Application.Images.DeleteImage;

public class DeleteImageCommandHandler:IRequestHandler<DeleteImageCommand, Result>
{
    private readonly IImageService _imageService;
    private readonly IRepository<Image> _imageRepository;

    public DeleteImageCommandHandler(IImageService imageService, IRepository<Image> imageRepository)
    {
        _imageService = imageService;
        _imageRepository = imageRepository;
    }

    public async Task<Result> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
    {
        var image = await _imageRepository.GetByIdAsync(request.ImageId,cancellationToken);
        if(image == null) return Result.Failure(ImageErrors.ImageNotFound());

        var result = await _imageService.DeleteImageAsync(image.Url);
        if(!result) return Result.Failure(ImageErrors.ErrorWhileDeleting()); 
        _imageRepository.Delete(image);
        await _imageRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}