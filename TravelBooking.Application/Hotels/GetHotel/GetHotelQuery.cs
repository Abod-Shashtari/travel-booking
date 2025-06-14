using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Hotels.GetHotel;

public record GetHotelQuery(Guid HotelId):IRequest<Result<HotelResponse?>>;