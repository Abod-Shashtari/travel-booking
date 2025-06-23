using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Bookings.GetBookings;

public class GetBookingsQueryHandler:IRequestHandler<GetBookingsQuery,Result<PaginatedList<BookingResponse>>>
{
    private readonly IRepository<Booking> _bookingRepository;
    private readonly IMapper _mapper;
    
    public GetBookingsQueryHandler(IRepository<Booking> bookingRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<BookingResponse>>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
    {
        var specification = new GetBookingsOfUserSpecification(request.UserId,request.PageNumber, request.PageSize,booking=>booking.CreatedAt,true);
        
        var bookings = await _bookingRepository.GetPaginatedListAsync(
            specification,
            cancellationToken
        );
        var mappedItems= _mapper.Map<List<BookingResponse>>(bookings.Data);
        var bookingsResponse= new PaginatedList<BookingResponse>(mappedItems, bookings.TotalCount, request.PageSize, request.PageNumber);
        
        return Result<PaginatedList<BookingResponse>>.Success(bookingsResponse);
    }
}