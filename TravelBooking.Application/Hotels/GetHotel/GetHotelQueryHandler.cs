using System.Linq.Expressions;
using System.Net.Mime;
using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.Images.Entities;

namespace TravelBooking.Application.Hotels.GetHotel;

public class GetHotelQueryHandler:IRequestHandler<GetHotelQuery, Result<HotelResponse?>>
{
    private readonly IRepository<Hotel> _hotelRepository;
    private readonly IRepository<Image> _imageRepository;
    private readonly IMapper _mapper;

    public GetHotelQueryHandler(IRepository<Hotel> hotelRepository, IMapper mapper, IRepository<Image> imageRepository)
    {
        _hotelRepository = hotelRepository;
        _mapper = mapper;
        _imageRepository = imageRepository;
    }

    public async Task<Result<HotelResponse?>> Handle(GetHotelQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Hotel, HotelResponse>> selector = hotel => new HotelResponse(
            hotel.Id,
            hotel.Name,
            hotel.Location,
            hotel.Description,
            hotel.City!.Name,
            hotel.CityId,
            hotel.OwnerId,
            hotel.CreatedAt,
            hotel.ModifiedAt
        );
        var hotel = await _hotelRepository.GetByIdAsync(request.HotelId,selector, cancellationToken);
        if (hotel == null) return Result<HotelResponse?>.Failure(HotelErrors.HotelNotFound());
        var hotelResponse = _mapper.Map<HotelResponse>(hotel);
        return Result<HotelResponse?>.Success(hotelResponse);
    }
}