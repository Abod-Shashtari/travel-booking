using MediatR;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.Errors;

namespace TravelBooking.Application.Rooms.DeleteRoom;

public class DeleteRoomCommandHandler:IRequestHandler<DeleteRoomCommand, Result>
{
    private readonly IRepository<Room> _roomRepository;
    
    public DeleteRoomCommandHandler(IRepository<Room> roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<Result> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var room= await _roomRepository.GetByIdAsync(request.RoomId,cancellationToken);
        if (room == null) return Result.Failure(RoomErrors.RoomNotFound());
        
        _roomRepository.Delete(room);
        await _roomRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}