using AutoMapper;
using MediatR;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.RoomTypes.Errors;

namespace TravelBooking.Application.RoomTypes.UpdateRoomType;

public class UpdateRoomTypeCommandHandler:IRequestHandler<UpdateRoomTypeCommand, Result>
{
    private readonly IMapper _mapper;
    private readonly IRepository<RoomType> _roomTypeRepository;
    private readonly IRepository<Hotel> _hotelRepository;
    
    public UpdateRoomTypeCommandHandler(IMapper mapper, IRepository<RoomType> roomTypeRepository, IRepository<Hotel> hotelRepository)
    {
        _mapper = mapper;
        _roomTypeRepository = roomTypeRepository;
        _hotelRepository = hotelRepository;
    }
    
    public async Task<Result> Handle(UpdateRoomTypeCommand request, CancellationToken cancellationToken)
    {
        if (!await _hotelRepository.IsExistsByIdAsync(request.HotelId,cancellationToken))
            return Result<Guid>.Failure(HotelErrors.HotelNotFound());
        
        var roomType = await _roomTypeRepository.GetByIdAsync(request.RoomTypeId,cancellationToken);
        if(roomType==null) return Result.Failure(RoomTypeErrors.RoomTypeNotFound());
        
        _mapper.Map(request,roomType);
        
        await _roomTypeRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}