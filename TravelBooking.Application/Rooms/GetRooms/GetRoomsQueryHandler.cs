using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Rooms.Specifications;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.RoomTypes.Errors;

namespace TravelBooking.Application.Rooms.GetRooms;

public class GetRoomsQueryHandler:IRequestHandler<GetRoomsQuery, Result<PaginatedList<RoomResponse>?>>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Room> _roomRepository;
    private readonly IRepository<RoomType> _roomTypeRepository;
    
    public GetRoomsQueryHandler(IMapper mapper, IRepository<Room> roomRepository, IRepository<RoomType> roomTypeRepository)
    {
        _mapper = mapper;
        _roomRepository = roomRepository;
        _roomTypeRepository = roomTypeRepository;
    }

    public async Task<Result<PaginatedList<RoomResponse>?>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
    {
        if(!await _roomTypeRepository.IsExistsByIdAsync(request.RoomTypeId, cancellationToken))
            return Result<PaginatedList<RoomResponse>?>.Failure(RoomTypeErrors.RoomTypeNotFound());
            
        var specification = new RoomsByRoomTypeIdSpecification(request.RoomTypeId,request.PageNumber, request.PageSize);
        var rooms = await _roomRepository.GetPaginatedListAsync(
            specification,
            cancellationToken
        );

        var mappedItems = _mapper.Map<List<RoomResponse>>(rooms.Data);
        var roomsResponse = new PaginatedList<RoomResponse>(mappedItems, rooms.TotalCount, request.PageSize, request.PageNumber);
        
        return Result<PaginatedList<RoomResponse>?>.Success(roomsResponse);
    }
}