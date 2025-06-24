using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Hotels.GetFeaturedHotels;

public record GetFeaturedHotelsQuery(): IRequest<Result<PaginatedList<HotelResponse>>>;