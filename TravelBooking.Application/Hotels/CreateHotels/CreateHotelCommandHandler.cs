using AutoMapper;
using MediatR;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;

namespace TravelBooking.Application.Hotels.CreateHotels;

public class CreateHotelCommandHandler:IRequestHandler<CreateHotelCommand, Result<Guid>>
{
    private readonly IRepository<Hotel> _hotelRepository;
    private readonly IMapper _mapper;

    public CreateHotelCommandHandler(IRepository<Hotel> hotelRepository, IMapper mapper)
    {
        _hotelRepository = hotelRepository;
        _mapper = mapper;
    }

    public async Task<Result<Guid>> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
    {
        var hotel=_mapper.Map<CreateHotelCommand, Hotel>(request);
        if(await _hotelRepository.IsExistAsync(hotel,cancellationToken))
            return Result<Guid>.Failure(HotelErrors.CityAlreadyExists());
        var hotelId = await _hotelRepository.AddAsync(hotel,cancellationToken);
        await _hotelRepository.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(hotelId);
    }
}