using System.ComponentModel.DataAnnotations;
using MediatR;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Hotels.CreateHotels;

public record CreateHotelCommand(
    [Required] string Name,
    [Required] Location Location,
    [Required] Guid CityId,
    [Required] Guid OwnerId
):IRequest<Result<Guid>>;