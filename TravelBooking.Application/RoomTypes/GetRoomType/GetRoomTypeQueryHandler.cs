using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.RoomTypes.Specifications;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.RoomTypes.Errors;

namespace TravelBooking.Application.RoomTypes.GetRoomType;

public class GetRoomTypeQueryHandler:IRequestHandler<GetRoomTypeQuery, Result<RoomTypeResponse?>>
{
    private readonly IRepository<RoomType> _roomTypeRepository;

    public GetRoomTypeQueryHandler(IRepository<RoomType> repository)
    {
        _roomTypeRepository = repository;
    }

    public async Task<Result<RoomTypeResponse?>> Handle(GetRoomTypeQuery request, CancellationToken cancellationToken)
    {
        var spec = new IncludeDiscountsWithRoomTypeSpecification();
        var roomType = await _roomTypeRepository.GetByIdAsync(request.RoomTypeId,spec,cancellationToken);
        if (roomType == null) return Result<RoomTypeResponse?>.Failure(RoomTypeErrors.RoomTypeNotFound());
        
        var discountedPrice=roomType.Discounts
            .Where(d => d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now)
            .Aggregate(
            roomType.PricePerNight,
            (price,discount ) => price*(1-discount.Percentage/100)
        );

        var roomTypeResponse = new RoomTypeResponse(
            roomType.Id,
            roomType.HotelId,
            roomType.Name,
            roomType.Description,
            roomType.PricePerNight,
            discountedPrice,
            roomType.CreatedAt,
            roomType.ModifiedAt
        );
        
        return Result<RoomTypeResponse?>.Success(roomTypeResponse);
    }
}