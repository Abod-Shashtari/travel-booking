using AutoMapper;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Images.Entities;

namespace TravelBooking.Application.Common.Profiles;

public class ImageProfile:Profile
{
    public ImageProfile()
    {
        CreateMap<Image, ImageFullResponse>();
    }
}