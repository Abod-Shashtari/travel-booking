using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Bookings.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.Errors;

namespace TravelBooking.Application.Bookings.CreateBooking;

public class CreateBookingCommandHandler:IRequestHandler<CreateBookingCommand, Result<BookingResponse?>>
{
    private readonly IRepository<Booking> _bookingRepository;
    private readonly IRepository<Hotel> _hotelRepository;
    private readonly IRepository<Room> _roomRepository;
    private readonly IMapper _mapper;
    
    public CreateBookingCommandHandler(IRepository<Booking> bookingRepository, IRepository<Hotel> hotelRepository, IRepository<Room> roomRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _hotelRepository = hotelRepository;
        _roomRepository = roomRepository;
        _mapper = mapper;
    }

    public async Task<Result<BookingResponse?>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        if (!await _hotelRepository.IsExistsByIdAsync(request.HotelId,cancellationToken))
            return Result<BookingResponse>.Failure(HotelErrors.HotelNotFound());

        var specification = new GetBookedRoomsSpecification(request.Rooms);
        var rooms = await _roomRepository.GetPaginatedListAsync(
            specification,
            cancellationToken
        );

        var days=(request.CheckOut-request.CheckIn).Days;
        decimal totalCost = 0;
        
        foreach (var room in rooms.Data)
        {
            var discountedPrice=room.RoomType!
                .Discounts
                .Where(d => d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now)
                .Aggregate(
                    room.RoomType.PricePerNight,
                    (price,discount ) => price*(1-discount.Percentage/100)
                );
            totalCost += discountedPrice*days;
        }
        
        if (rooms.TotalCount != request.Rooms.Count)
            return Result<BookingResponse>.Failure(RoomErrors.RoomNotFound());

        var booking=_mapper.Map<Booking>(request);
        booking.Rooms = rooms.Data;
        booking.Status = BookingStatus.Pending;
        booking.TotalCost = totalCost;
        
        if(await _bookingRepository.IsExistAsync(booking,cancellationToken))
            return Result<BookingResponse>.Failure(BookingErrors.BookingConflict());
        
        await _bookingRepository.AddAsync(booking,cancellationToken);
        await _bookingRepository.SaveChangesAsync(cancellationToken);

        var bookingResponse = _mapper.Map<BookingResponse>(booking)
            with {
                RoomIds = request.Rooms,
                Status = booking.Status.ToString()
            };
        
        return Result<BookingResponse?>.Success(bookingResponse);
    }
}