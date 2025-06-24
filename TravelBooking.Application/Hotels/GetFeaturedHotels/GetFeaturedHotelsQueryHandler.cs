using System.Linq.Expressions;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Hotels.GetFeaturedHotels;

public class GetFeaturedHotelsQueryHandler:IRequestHandler<GetFeaturedHotelsQuery,Result<PaginatedList<HotelResponse>>>
{
    private readonly IRepository<Hotel> _hotelRepository;

    public GetFeaturedHotelsQueryHandler(IRepository<Hotel> hotelRepository)
    {
        _hotelRepository = hotelRepository;
    }

    public async Task<Result<PaginatedList<HotelResponse>>> Handle(GetFeaturedHotelsQuery request, CancellationToken cancellationToken)
    {
        var specification = new GetFeaturedHotelsSpecification(5);
        
        Expression<Func<Hotel, HotelResponse>> selector = hotel => new HotelResponse(
            hotel.Id,
            hotel.Name,
            hotel.Location,
            hotel.Description,
            hotel.City!.Name,
            hotel.CityId,
            hotel.OwnerId,
            hotel.CreatedAt,
            hotel.ModifiedAt,
            new ImageResponse(
                hotel.ThumbnailImageId,
                hotel.ThumbnailImage != null ? hotel.ThumbnailImage.Url : ""
            )
        );
        
        var hotels = await _hotelRepository.GetPaginatedListAsync(
            specification,
            selector,
            cancellationToken
        );
        
        return Result<PaginatedList<HotelResponse>>.Success(hotels);
    }
}