using System.Linq.Expressions;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.UserActivity.Specifications;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.UserActivity.Entites;

namespace TravelBooking.Application.UserActivity.GetRecentlyVisitedHotels;

public class GetRecentlyVisitedHotelsQueryHandler:IRequestHandler<GetRecentlyVisitedHotelsQuery,Result<PaginatedList<HotelResponse>>>
{
    private readonly IRepository<HotelVisit> _hotelVisitRepository;

    public GetRecentlyVisitedHotelsQueryHandler(IRepository<HotelVisit> hotelVisitRepository)
    {
        _hotelVisitRepository = hotelVisitRepository;
    }

    public async Task<Result<PaginatedList<HotelResponse>>> Handle(GetRecentlyVisitedHotelsQuery request, CancellationToken cancellationToken)
    {
        var specification = new GetRecentlyVisitedHotelsSpecification(request.UserId,5);
        
        Expression<Func<HotelVisit, HotelResponse>> selector = hotelVisit => new HotelResponse(
            hotelVisit.HotelId,
            hotelVisit.Hotel!.Name,
            hotelVisit.Hotel!.Location,
            hotelVisit.Hotel!.Description,
            hotelVisit.Hotel!.City!.Name,
            hotelVisit.Hotel!.CityId,
            hotelVisit.Hotel!.OwnerId,
            hotelVisit.Hotel!.StarRating,
            hotelVisit.Hotel!.CreatedAt,
            hotelVisit.Hotel!.ModifiedAt,
            new ImageResponse(
                hotelVisit.Hotel!.ThumbnailImageId,
                hotelVisit.Hotel!.ThumbnailImage != null ? hotelVisit.Hotel!.ThumbnailImage.Url : ""
            )
        );
        
        var hotels = await _hotelVisitRepository.GetPaginatedListAsync(
            specification,
            selector,
            cancellationToken
        );
        
        return Result<PaginatedList<HotelResponse>>.Success(hotels);
    }
}