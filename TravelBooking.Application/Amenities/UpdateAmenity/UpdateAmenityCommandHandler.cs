using AutoMapper;
using MediatR;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Amenities.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Amenities.UpdateAmenity;

public class UpdateAmenityCommandHandler:IRequestHandler<UpdateAmenityCommand,Result>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Amenity> _amenityRepository;
    
    public UpdateAmenityCommandHandler(IMapper mapper, IRepository<Amenity> amenityRepository)
    {
        _mapper = mapper;
        _amenityRepository = amenityRepository;
    }
    public async Task<Result> Handle(UpdateAmenityCommand request, CancellationToken cancellationToken)
    {
        var amenity = await _amenityRepository.GetByIdAsync(request.AmenityId,cancellationToken);
        if(amenity==null) return Result.Failure(AmenityErrors.AmenityNotFound());
        
        _mapper.Map(request,amenity);
        await _amenityRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}