using System.Linq.Expressions;
using MediatR;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Hotels.SearchHotel;

public class SearchHotelQueryHandler:IRequestHandler<SearchHotelQuery, Result<PaginatedList<SearchHotelResponse>>>
{
    private readonly IRepository<Hotel> _hotelRepository;
    public SearchHotelQueryHandler(IRepository<Hotel> hotelRepository)
    {
        _hotelRepository = hotelRepository;
    }

    public async Task<Result<PaginatedList<SearchHotelResponse>>> Handle(SearchHotelQuery request, CancellationToken cancellationToken)
    {
        var spec = new HotelSearchSpecification(request.HotelFilter);
        
        Expression<Func<Hotel, SearchHotelResponse>> selector = hotel => new SearchHotelResponse(
            hotel.Id,
            hotel.Name,
            hotel.Location,
            hotel.City.Id,
            hotel.City.Name
        );
        
        var hotels = await _hotelRepository.GetPaginatedListAsync(
            spec,
            selector,
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );
        
        return Result<PaginatedList<SearchHotelResponse>>.Success(hotels);
    }
}