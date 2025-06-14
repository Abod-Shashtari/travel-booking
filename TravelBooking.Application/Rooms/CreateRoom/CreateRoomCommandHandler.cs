using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.Errors;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.RoomTypes.Errors;

namespace TravelBooking.Application.Rooms.CreateRoom;

public class CreateRoomCommandHandler:IRequestHandler<CreateRoomCommand, Result<RoomResponse?>>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Room> _roomRepository;
    private readonly IRepository<RoomType> _roomTypeRepository;
    
    public CreateRoomCommandHandler(IMapper mapper, IRepository<Room> hotelRepository, IRepository<RoomType> roomTypeRepository)
    {
        _mapper = mapper;
        _roomRepository = hotelRepository;
        _roomTypeRepository = roomTypeRepository;
    }

    public async Task<Result<RoomResponse?>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        if(!await _roomTypeRepository.IsExistsByIdAsync(request.RoomTypeId,cancellationToken))
            return Result<RoomResponse>.Failure(RoomTypeErrors.RoomTypeNotFound());
        
        var room=_mapper.Map<CreateRoomCommand, Room>(request);
        if(await _roomRepository.IsExistAsync(room,cancellationToken))
            return Result<RoomResponse>.Failure(RoomErrors.RoomAlreadyExists());
        
        await _roomRepository.AddAsync(room,cancellationToken);
        await _roomRepository.SaveChangesAsync(cancellationToken);
        var roomResponse=_mapper.Map<RoomResponse>(room);
        return Result<RoomResponse?>.Success(roomResponse);
    }
}