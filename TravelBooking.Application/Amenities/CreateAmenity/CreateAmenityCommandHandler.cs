using AutoMapper;
using MediatR;
using TravelBooking.Application.Cities.CreateCity;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Amenities.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Amenities.CreateAmenity;

public class CreateAmenityCommandHandler:IRequestHandler<CreateAmenityCommand,Result<AmenityResponse?>>
{
    private readonly IRepository<Amenity> _amenityRepository;
    private readonly IMapper _mapper;
    
    public CreateAmenityCommandHandler(IRepository<Amenity> amenityRepository, IMapper mapper)
    {
        _amenityRepository = amenityRepository;
        _mapper = mapper;
    }

    public async Task<Result<AmenityResponse?>> Handle(CreateAmenityCommand request, CancellationToken cancellationToken)
    {
        var amenity = _mapper.Map<Amenity>(request);
        if (await _amenityRepository.IsExistAsync(amenity, cancellationToken))
            return Result<AmenityResponse>.Failure(AmenityErrors.AmenityAlreadyExists());
        await _amenityRepository.AddAsync(amenity,cancellationToken);
        await _amenityRepository.SaveChangesAsync(cancellationToken);
        var amenityResponse = _mapper.Map<AmenityResponse>(amenity);
        return Result<AmenityResponse?>.Success(amenityResponse);
    }
}