using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.Errors;

namespace TravelBooking.Application.Rooms.GetRoom;

public class GetRoomQueryHandler:IRequestHandler<GetRoomQuery,Result<RoomResponse?>>
{
    private readonly IRepository<Room> _roomRepository;
    private readonly IMapper _mapper;
    
    public GetRoomQueryHandler(IRepository<Room> roomRepository, IMapper mapper)
    {
        _roomRepository = roomRepository;
        _mapper = mapper;
    }
    public async Task<Result<RoomResponse?>> Handle(GetRoomQuery request, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room == null) return Result<RoomResponse?>.Failure(RoomErrors.RoomNotFound());
        var roomResponse = _mapper.Map<RoomResponse>(room);
        return Result<RoomResponse?>.Success(roomResponse);
    }
}