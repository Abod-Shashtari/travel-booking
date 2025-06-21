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

    public GetHotelQueryHandler(IRepository<Hotel> hotelRepository)
    {
        _hotelRepository = hotelRepository;
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
        var hotelResponse = await _hotelRepository.GetByIdAsync(request.HotelId,selector, cancellationToken);
        if (hotelResponse == null) return Result<HotelResponse?>.Failure(HotelErrors.HotelNotFound());
        return Result<HotelResponse?>.Success(hotelResponse);
    }
}