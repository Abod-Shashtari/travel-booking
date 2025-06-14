using MediatR;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Hotels.UpdateHotel;

public record UpdateHotelCommand(
    Guid HotelId,
    string Name,
    string? Description,
    Location Location,
    Guid CityId,
    Guid OwnerId
):IRequest<Result>;
