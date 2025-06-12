using AutoMapper;
using MediatR;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.RoomTypes.Errors;

namespace TravelBooking.Application.RoomTypes.DeleteRoomType;

public class DeleteRoomTypeCommandHandler: IRequestHandler<DeleteRoomTypeCommand,Result>
{
    private readonly IRepository<RoomType> _roomTypeRepository;
    
    public DeleteRoomTypeCommandHandler(IRepository<RoomType> roomTypeRepository)
    {
        _roomTypeRepository = roomTypeRepository;
    }
    public async Task<Result> Handle(DeleteRoomTypeCommand request, CancellationToken cancellationToken)
    {
        var roomType = await _roomTypeRepository.GetByIdAsync(request.RoomTypeId,cancellationToken);
        if(roomType==null) return Result.Failure(RoomTypeErrors.RoomTypeNotFound());
        
        _roomTypeRepository.Delete(roomType);
        await _roomTypeRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}