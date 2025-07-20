using MediatR;
using TravelBooking.Application.Common.Extensions;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.RoomTypes.Specifications;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.RoomTypes.Entities;

namespace TravelBooking.Application.RoomTypes.GetRoomTypesOfHotel;

public class GetRoomTypesOfHotelQueryHandler:IRequestHandler<GetRoomTypesOfHotelQuery, Result<PaginatedList<RoomTypeResponse>?>>
{
    private readonly IRepository<RoomType> _roomTypeRepository;
    private readonly IRepository<Hotel> _hotelRepository;

    public GetRoomTypesOfHotelQueryHandler(IRepository<RoomType> repository, IRepository<Hotel> hotelRepository)
    {
        _roomTypeRepository = repository;
        _hotelRepository = hotelRepository;
    }

    public async Task<Result<PaginatedList<RoomTypeResponse>?>> Handle(GetRoomTypesOfHotelQuery request, CancellationToken cancellationToken)
    {
        if(!await _hotelRepository.IsExistsByIdAsync(request.HotelId,cancellationToken))
            return Result<PaginatedList<RoomTypeResponse>>.Failure(HotelErrors.HotelNotFound());
        var spec = new HotelRoomTypesWithDiscountsSpecification(request.HotelId, request.PageNumber, request.PageSize);
        
        var roomTypes= await _roomTypeRepository.GetPaginatedListAsync(
            spec,
            cancellationToken
        );
        
        var roomTypesResponse = roomTypes.Data.Select(
            rt => new RoomTypeResponse(
                rt.Id,
                rt.HotelId,
                rt.Name,
                rt.Description,
                rt.PricePerNight,
                CalculateDiscountedPrice(rt.Discounts,rt.PricePerNight),
                rt.CreatedAt,
                rt.ModifiedAt
            )
        ).ToList();
        var pagedRoomTypeResponse = roomTypes.Map(roomTypesResponse);
        return Result<PaginatedList<RoomTypeResponse>?>.Success(pagedRoomTypeResponse);
    }

    private decimal CalculateDiscountedPrice(ICollection<Discount> discounts, decimal pricePerNight)
    {
        return discounts.Where(d => d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now)
            .Aggregate(pricePerNight, (price, discount) => price * (1 - discount.Percentage / 100));
    }
}