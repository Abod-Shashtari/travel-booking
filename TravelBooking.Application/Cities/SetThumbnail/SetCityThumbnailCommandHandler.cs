using MediatR;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Cities.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Images.Errors;

namespace TravelBooking.Application.Cities.SetThumbnail;

public class SetCityThumbnailCommandHandler:IRequestHandler<SetCityThumbnailCommand, Result>
{
    private readonly IRepository<City> _cityRepository;
    private readonly IRepository<Image> _imageRepository;

    public SetCityThumbnailCommandHandler(IRepository<City> cityRepository, IRepository<Image> imageRepository)
    {
        _cityRepository = cityRepository;
        _imageRepository = imageRepository;
    }

    public async Task<Result> Handle(SetCityThumbnailCommand request, CancellationToken cancellationToken)
    {
        if (!await _imageRepository.IsExistsByIdAsync(request.ImageId, cancellationToken))
            return Result.Failure(ImageErrors.ImageNotFound());
        
        var city = await _cityRepository.GetByIdAsync(request.CityId, cancellationToken);
        if (city == null) return Result.Failure(CityErrors.CityNotFound());
        
        city.ThumbnailImageId = request.ImageId;
        await _cityRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}