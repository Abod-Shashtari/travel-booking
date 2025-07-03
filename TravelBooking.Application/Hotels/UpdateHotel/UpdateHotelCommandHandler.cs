using AutoMapper;
using MediatR;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Cities.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.Users.Errors;
using TravelBooking.Domain.Users.Interfaces;

namespace TravelBooking.Application.Hotels.UpdateHotel;

public class UpdateHotelCommandHandler:IRequestHandler<UpdateHotelCommand, Result>
{
    private readonly IRepository<Hotel> _hotelRepository;
    private readonly IRepository<City> _citiesRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UpdateHotelCommandHandler(IRepository<Hotel> hotelRepository, IMapper mapper, IUserRepository userRepository, IRepository<City> citiesRepository)
    {
        _hotelRepository = hotelRepository;
        _mapper = mapper;
        _userRepository = userRepository;
        _citiesRepository = citiesRepository;
    }

    public async Task<Result> Handle(UpdateHotelCommand request, CancellationToken cancellationToken)
    {
        if (!await _citiesRepository.IsExistsByIdAsync(request.CityId,cancellationToken))
            return Result<Guid>.Failure(CityErrors.CityNotFound());
        if (!await _userRepository.IsExistsByIdAsync(request.OwnerId,cancellationToken))
            return Result<Guid>.Failure(UserErrors.UserNotFound());
        
        var hotel = await _hotelRepository.GetByIdAsync(request.HotelId,cancellationToken);
        if(hotel==null) return Result.Failure(HotelErrors.HotelNotFound());
        
        _mapper.Map(request,hotel);
        
        await _hotelRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}