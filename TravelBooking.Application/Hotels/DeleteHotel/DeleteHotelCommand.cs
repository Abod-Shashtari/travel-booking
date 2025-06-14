using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Hotels.DeleteHotel;

public record DeleteHotelCommand(Guid HotelId):IRequest<Result>;