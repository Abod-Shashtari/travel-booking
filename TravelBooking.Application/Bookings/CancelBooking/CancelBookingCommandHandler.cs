using MediatR;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Bookings.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Users.Errors;

namespace TravelBooking.Application.Bookings.CancelBooking;

public class CancelBookingCommandHandler:IRequestHandler<CancelBookingCommand,Result>
{
    private readonly IRepository<Booking> _bookingRepository;

    public CancelBookingCommandHandler(IRepository<Booking> bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }


    public async Task<Result> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId,cancellationToken);
        if (booking == null)
            return Result.Failure(BookingErrors.BookingNotFound());
        
        if(request.UserId!=booking.UserId)
            return Result.Failure(BookingErrors.UserNotOwnerOfBooking());

        if(booking.Status == BookingStatus.Pending)
            booking.Status = BookingStatus.Cancelled;
        else return Result.Failure(BookingErrors.BookingUncancellable());
        
        await _bookingRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}