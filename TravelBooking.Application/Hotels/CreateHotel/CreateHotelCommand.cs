using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Hotels.CreateHotel;

public record CreateHotelCommand(
    string Name,
    string? Description,
    Location Location,
    Guid CityId,
    Guid OwnerId
):IRequest<Result<HotelResponse?>>;