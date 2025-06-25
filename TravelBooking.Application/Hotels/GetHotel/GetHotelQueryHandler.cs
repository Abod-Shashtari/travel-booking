using System.Linq.Expressions;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Hotels.Specifications;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.UserActivity.Entites;

namespace TravelBooking.Application.Hotels.GetHotel;

public class GetHotelQueryHandler:IRequestHandler<GetHotelQuery, Result<HotelResponse?>>
{
    private readonly IRepository<Hotel> _hotelRepository;
    private readonly IRepository<HotelVisit> _hotelVisitRepository;

    public GetHotelQueryHandler(IRepository<Hotel> hotelRepository, IRepository<HotelVisit> hotelVisitRepository)
    {
        _hotelRepository = hotelRepository;
        _hotelVisitRepository = hotelVisitRepository;
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
            hotel.ModifiedAt,
            new ImageResponse(
                hotel.ThumbnailImageId,
                hotel.ThumbnailImage != null ? hotel.ThumbnailImage.Url : ""
            )
        );
        var hotelResponse = await _hotelRepository.GetByIdAsync(request.HotelId,selector, cancellationToken);
        if (hotelResponse == null) return Result<HotelResponse?>.Failure(HotelErrors.HotelNotFound());
        
        var specification = new GetHotelVisitedSpecification(hotelResponse.Id,request.VisitorUserId);
        var existedHotelVisitRecord=await _hotelVisitRepository.GetPaginatedListAsync(specification,cancellationToken);
        existedHotelVisitRecord.Data.ForEach(hv=>_hotelVisitRepository.Delete(hv));
        
        var hotelVisit = new HotelVisit {HotelId = hotelResponse.Id,UserId = request.VisitorUserId};
        await _hotelVisitRepository.AddAsync(hotelVisit,cancellationToken);
        await _hotelVisitRepository.SaveChangesAsync(cancellationToken);
        
        return Result<HotelResponse?>.Success(hotelResponse);
    }
}