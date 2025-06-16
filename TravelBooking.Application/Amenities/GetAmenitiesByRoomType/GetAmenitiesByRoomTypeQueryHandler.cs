using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Amenities.GetAmenitiesByRoomType;

public class GetAmenitiesByRoomTypeQueryHandler:IRequestHandler<GetAmenitiesByRoomTypeQuery, Result<PaginatedList<AmenityResponse>>>
{
    private readonly IRepository<Amenity> _amenityRepository;

    public GetAmenitiesByRoomTypeQueryHandler(IRepository<Amenity> amenityRepository)
    {
        _amenityRepository = amenityRepository;
    }
    
    public async Task<Result<PaginatedList<AmenityResponse>>> Handle(GetAmenitiesByRoomTypeQuery request, CancellationToken cancellationToken)
    {
        var specification = new AmenitiesByRoomTypeSpecification(request.RoomTypeId);
        var amenities=await _amenityRepository.GetPaginatedListAsync(
            specification,
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );
        
        var amenitiesResponse = amenities.Data.Select(
            a => new AmenityResponse(
                a.Id,
                a.Name,
                a.Description,
                a.CreatedAt,
                a.ModifiedAt
            )
        ).ToList();
        var pagedAmenitiesResponse = amenities.Map(amenitiesResponse);
        return Result<PaginatedList<AmenityResponse>>.Success(pagedAmenitiesResponse);
    }
}