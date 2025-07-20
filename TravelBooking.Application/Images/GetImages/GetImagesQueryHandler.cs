using System.Linq.Expressions;
using MediatR;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Images.Specifications;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Images.Errors;
using TravelBooking.Domain.Images.Interfaces;

namespace TravelBooking.Application.Images.GetImages;

public class GetImagesQueryHandler:IRequestHandler<GetImagesQuery, Result<PaginatedList<ImageResponse>?>>
{
    private readonly IImageRepository _imageRepository;
    
    public GetImagesQueryHandler(IImageRepository imageRepository)
    {
        _imageRepository = imageRepository;
    }

    public async Task<Result<PaginatedList<ImageResponse>?>> Handle(GetImagesQuery request, CancellationToken cancellationToken)
    {
        var requestEntityName=request.EntityName.Replace("-","");
        if (!Enum.TryParse<EntityType>(requestEntityName, ignoreCase: true, out var entityType))
            return Result<PaginatedList<ImageResponse>?>.Failure(ImageErrors.ImageInvalidEntityType());
        
        Expression<Func<Image, bool>> criteria = image =>
            image.EntityType == entityType && image.EntityId == request.EntityId;
        
        ISpecification<Image> spec = new ImageSpecification(criteria,request.PageSize,request.PageNumber);
        
        var imagesPaginated= await _imageRepository.GetPaginatedListAsync(
            spec,
            selector: img => new ImageResponse(img.Id,img.Url),
            cancellationToken: cancellationToken
        );
        
        return Result<PaginatedList<ImageResponse>?>.Success(imagesPaginated);
    }
}