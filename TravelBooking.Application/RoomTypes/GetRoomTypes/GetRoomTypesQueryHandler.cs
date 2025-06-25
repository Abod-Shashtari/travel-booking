using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.RoomTypes.Specifications;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.RoomTypes.Entities;

namespace TravelBooking.Application.RoomTypes.GetRoomTypes;

public class GetRoomTypesQueryHandler:IRequestHandler<GetRoomTypesQuery, Result<PaginatedList<RoomTypeResponse>>>
{
    private readonly IRepository<RoomType> _roomTypeRepository;

    public GetRoomTypesQueryHandler(IRepository<RoomType> repository)
    {
        _roomTypeRepository = repository;
    }

    public async Task<Result<PaginatedList<RoomTypeResponse>>> Handle(GetRoomTypesQuery request, CancellationToken cancellationToken)
    {
        var spec = new RoomTypeWithDiscountsSpecification(request.PageNumber, request.PageSize);
        
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
        return Result<PaginatedList<RoomTypeResponse>>.Success(pagedRoomTypeResponse);
    }

    private decimal CalculateDiscountedPrice(ICollection<Discount> discounts, decimal pricePerNight)
    {
        return discounts.Where(d => d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now)
            .Aggregate(pricePerNight, (price, discount) => price * (1 - discount.Percentage / 100));
    }
}