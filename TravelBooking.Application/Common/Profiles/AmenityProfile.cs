using AutoMapper;
using TravelBooking.Application.Amenities.CreateAmenity;
using TravelBooking.Application.Amenities.UpdateAmenity;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Amenities.Entities;

namespace TravelBooking.Application.Common.Profiles;

public class AmenityProfile:Profile
{
    public AmenityProfile()
    {
        CreateMap<CreateAmenityCommand, Amenity>();
        CreateMap<UpdateAmenityCommand, Amenity>();
        CreateMap<Amenity,AmenityResponse>();
    }
}