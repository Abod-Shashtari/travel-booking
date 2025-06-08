using System.ComponentModel.DataAnnotations;
using MediatR;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Hotels.CreateHotels;

public record CreateHotelCommand(
    string Name,
    Location Location,
    Guid CityId,
    Guid OwnerId
):IRequest<Result<Guid>>;