using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.RoomTypes.Errors;

namespace TravelBooking.Application.RoomTypes.CreateRoomType;

public class CreateRoomTypeCommandHandler:IRequestHandler<CreateRoomTypeCommand,Result<RoomTypeResponse?>>
{
    private readonly IMapper _mapper;
    private readonly IRepository<RoomType> _roomTypeRepository;
    private readonly IRepository<Hotel> _hotelRepository;
    
    public CreateRoomTypeCommandHandler(IMapper mapper, IRepository<RoomType> roomTypeRepository, IRepository<Hotel> hotelRepository)
    {
        _mapper = mapper;
        _roomTypeRepository = roomTypeRepository;
        _hotelRepository = hotelRepository;
    }

    public async Task<Result<RoomTypeResponse?>> Handle(CreateRoomTypeCommand request, CancellationToken cancellationToken)
    {
        if (!await _hotelRepository.IsExistsByIdAsync(request.HotelId,cancellationToken))
            return Result<RoomTypeResponse?>.Failure(HotelErrors.HotelNotFound());
        
        var roomType=_mapper.Map<CreateRoomTypeCommand, RoomType>(request);
        if(await _roomTypeRepository.IsExistAsync(roomType,cancellationToken))
            return Result<RoomTypeResponse>.Failure(RoomTypeErrors.RoomTypeAlreadyExists());
        
        await _roomTypeRepository.AddAsync(roomType,cancellationToken);
        await _roomTypeRepository.SaveChangesAsync(cancellationToken);
        
        var roomTypeResponse=_mapper.Map<RoomTypeResponse>(roomType);
        return Result<RoomTypeResponse?>.Success(roomTypeResponse);
    }
}