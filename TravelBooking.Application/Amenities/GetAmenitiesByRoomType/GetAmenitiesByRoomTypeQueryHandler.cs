using AutoMapper;
using MediatR;
using TravelBooking.Application.Amenities.Specifications;
using TravelBooking.Application.Common.Extensions;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Amenities.GetAmenitiesByRoomType;

public class GetAmenitiesByRoomTypeQueryHandler:IRequestHandler<GetAmenitiesByRoomTypeQuery, Result<PaginatedList<AmenityResponse>>>
{
    private readonly IRepository<Amenity> _amenityRepository;
    private readonly IMapper _mapper;

    public GetAmenitiesByRoomTypeQueryHandler(IRepository<Amenity> amenityRepository, IMapper mapper)
    {
        _amenityRepository = amenityRepository;
        _mapper = mapper;
    }
    
    public async Task<Result<PaginatedList<AmenityResponse>>> Handle(GetAmenitiesByRoomTypeQuery request, CancellationToken cancellationToken)
    {
        var specification = new AmenitiesByRoomTypeSpecification(request.RoomTypeId,request.PageNumber, request.PageSize);
        var amenities=await _amenityRepository.GetPaginatedListAsync(
            specification,
            cancellationToken
        );
        
        var mappedItems = _mapper.Map<List<AmenityResponse>>(amenities.Data);
        var amenitiesResponse = amenities.Map(mappedItems);
        return Result<PaginatedList<AmenityResponse>>.Success(amenitiesResponse);
    }
}