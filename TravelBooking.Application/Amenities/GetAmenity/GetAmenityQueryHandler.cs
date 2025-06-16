using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Amenities.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Amenities.GetAmenity;

public class GetAmenityQueryHandler:IRequestHandler<GetAmenityQuery,Result<AmenityResponse?>>
{
    private readonly IRepository<Amenity> _amenityRepository;
    private readonly IMapper _mapper;
    
    public GetAmenityQueryHandler(IRepository<Amenity> amenityRepository, IMapper mapper)
    {
        _amenityRepository = amenityRepository;
        _mapper = mapper;
    }
    public async Task<Result<AmenityResponse?>> Handle(GetAmenityQuery request, CancellationToken cancellationToken)
    {
        var amenity = await _amenityRepository.GetByIdAsync(request.AmenityId, cancellationToken);
        if (amenity == null) return Result<AmenityResponse?>.Failure(AmenityErrors.AmenityNotFound());
        var amenityResponse = _mapper.Map<AmenityResponse>(amenity);
        return Result<AmenityResponse?>.Success(amenityResponse);
    }
}