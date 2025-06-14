using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.Errors;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.RoomTypes.Errors;

namespace TravelBooking.Application.Rooms.UpdateRoom;

public class UpdateRoomCommandHandler:IRequestHandler<UpdateRoomCommand,Result>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Room> _roomRepository;
    private readonly IRepository<RoomType> _roomTypeRepository;
    
    public UpdateRoomCommandHandler(IMapper mapper, IRepository<Room> roomRepository, IRepository<RoomType> roomTypeRepository)
    {
        _mapper = mapper;
        _roomRepository = roomRepository;
        _roomTypeRepository = roomTypeRepository;
    }
    public async Task<Result> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        if(!await _roomTypeRepository.IsExistsByIdAsync(request.RoomTypeId,cancellationToken))
            return Result<RoomResponse>.Failure(RoomTypeErrors.RoomTypeAlreadyExists());
        
        var room = await _roomRepository.GetByIdAsync(request.RoomId,cancellationToken);
        if(room==null) return Result.Failure(RoomErrors.RoomDoesNotExists());
        
        _mapper.Map(request,room);
        await _roomRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}