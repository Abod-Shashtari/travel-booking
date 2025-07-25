﻿using MediatR;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.Images.Errors;
using TravelBooking.Domain.Images.Interfaces;

namespace TravelBooking.Application.Hotels.SetThumbnail;

public class SetHotelThumbnailCommandHandler:IRequestHandler<SetHotelThumbnailCommand, Result>
{
    private readonly IRepository<Hotel> _hotelRepository;
    private readonly IImageRepository _imageRepository;

    public SetHotelThumbnailCommandHandler(IRepository<Hotel> hotelRepository, IImageRepository imageRepository)
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
        await _hotelRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}