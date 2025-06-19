using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Amenities.GetAmenities;

public class GetAmenitiesQueryHandler:IRequestHandler<GetAmenitiesQuery, Result<PaginatedList<AmenityResponse>>>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Amenity> _amenityRepository;

    public GetAmenitiesQueryHandler(IMapper mapper, IRepository<Amenity> amenityRepository)
    {
        _mapper = mapper;
        _amenityRepository = amenityRepository;
    }

    public async Task<Result<PaginatedList<AmenityResponse>>> Handle(GetAmenitiesQuery request, CancellationToken cancellationToken)
    {
        var specification = new PaginationSpecification<Amenity>(request.PageNumber, request.PageSize,amenity=>amenity.CreatedAt,true);
        var amenities= await _amenityRepository.GetPaginatedListAsync(
            specification,
            cancellationToken
        );

        var mappedItems = _mapper.Map<List<AmenityResponse>>(amenities.Data);
        var amenitiesResponse = new PaginatedList<AmenityResponse>(mappedItems, amenities.TotalCount, request.PageSize, request.PageNumber);
        
        return Result<PaginatedList<AmenityResponse>>.Success(amenitiesResponse);
    }
}