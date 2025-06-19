using MediatR;
using TravelBooking.Application.Cities.SetThumbnail;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Images.Errors;

namespace TravelBooking.Application.Hotels.SetThumbnail;

public class SetHotelThumbnailCommandHandler:IRequestHandler<SetHotelThumbnailCommand, Result>
{
    private readonly IRepository<Hotel> _hotelRepository;
    private readonly IRepository<Image> _imageRepository;

    public SetHotelThumbnailCommandHandler(IRepository<Hotel> hotelRepository, IRepository<Image> imageRepository)
    {
        _hotelRepository = hotelRepository;
        _imageRepository = imageRepository;
    }

    public async Task<Result> Handle(SetHotelThumbnailCommand request, CancellationToken cancellationToken)
    {
        if (!await _imageRepository.IsExistsByIdAsync(request.ImageId, cancellationToken))
            return Result.Failure(ImageErrors.ImageNotFound());
        
        var hotel = await _hotelRepository.GetByIdAsync(request.HotelId, cancellationToken);
        if (hotel == null) return Result.Failure(HotelErrors.HotelNotFound());
        
        hotel.ThumbnailImageId = request.ImageId;
        return Result.Success();
    }
}