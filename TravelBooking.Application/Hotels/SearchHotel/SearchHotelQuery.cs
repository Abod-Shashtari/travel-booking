using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Hotels.SearchHotel;

public record SearchHotelQuery(HotelFilter? HotelFilter,int PageNumber, int PageSize):IRequest<Result<PaginatedList<SearchHotelResponse>>>;