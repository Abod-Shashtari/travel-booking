using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Cities.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.Users.Errors;
using TravelBooking.Domain.Users.Interfaces;

namespace TravelBooking.Application.Hotels.CreateHotels;

public class CreateHotelCommandHandler:IRequestHandler<CreateHotelCommand, Result<HotelResponse?>>
{
    private readonly IRepository<Hotel> _hotelRepository;
    private readonly IRepository<City> _citiesRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public CreateHotelCommandHandler(IRepository<Hotel> hotelRepository, IMapper mapper, IRepository<City> citiesRepository, IUserRepository userRepository)
    {
        _hotelRepository = hotelRepository;
        _mapper = mapper;
        _citiesRepository = citiesRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<HotelResponse?>> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
    {
        if (!await _citiesRepository.IsExistsByIdAsync(request.CityId,cancellationToken))
            return Result<HotelResponse>.Failure(CityErrors.CityNotFound());
        if (!await _userRepository.IsExistsByIdAsync(request.OwnerId,cancellationToken))
            return Result<HotelResponse>.Failure(UserErrors.UserNotFound());
        
        var hotel=_mapper.Map<CreateHotelCommand, Hotel>(request);
        if(await _hotelRepository.IsExistAsync(hotel,cancellationToken))
            return Result<HotelResponse>.Failure(HotelErrors.HotelAlreadyExists());
        await _hotelRepository.AddAsync(hotel,cancellationToken);
        await _hotelRepository.SaveChangesAsync(cancellationToken);
        
        var hotelResponse=_mapper.Map<HotelResponse>(hotel);
        return Result<HotelResponse?>.Success(hotelResponse);
    }
}