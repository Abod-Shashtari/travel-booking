using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Rooms.Specifications;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Application.Rooms.GetRooms;

public class GetRoomsQueryHandler:IRequestHandler<GetRoomsQuery, Result<PaginatedList<RoomResponse>>>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Room> _roomRepository;
    
    public GetRoomsQueryHandler(IMapper mapper, IRepository<Room> roomRepository)
    {
        _mapper = mapper;
        _roomRepository = roomRepository;
    }

    public async Task<Result<PaginatedList<RoomResponse>>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
    {
        var specification = new RoomsByRoomTypeIdSpecification(request.RoomTypeId,request.PageNumber, request.PageSize);
        var rooms = await _roomRepository.GetPaginatedListAsync(
            specification,
            cancellationToken
        );

        var mappedItems = _mapper.Map<List<RoomResponse>>(rooms.Data);
        var roomsResponse = new PaginatedList<RoomResponse>(mappedItems, rooms.TotalCount, request.PageSize, request.PageNumber);
        
        return Result<PaginatedList<RoomResponse>>.Success(roomsResponse);
    }
}