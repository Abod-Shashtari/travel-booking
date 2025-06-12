using System.Linq.Expressions;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
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
        Expression<Func<RoomType, RoomTypeResponse>> selector = roomType => new RoomTypeResponse(
            roomType.Id,
            roomType.HotelId,
            roomType.Name,
            roomType.Description,
            roomType.PricePerNight,
            roomType.PricePerNight,
            roomType.CreatedAt,
            roomType.ModifiedAt
        );
        
        var roomTypes= await _roomTypeRepository.GetPaginatedListAsync(
            null,
            selector,
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );
        
        return Result<PaginatedList<RoomTypeResponse>>.Success(roomTypes);
    }
}