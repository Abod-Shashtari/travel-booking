using MediatR;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Bookings.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Bookings.ConfirmBooking;

public class ConfirmBookingCommandHandler:IRequestHandler<ConfirmBookingCommand,Result>
{
    private readonly IRepository<Booking> _bookingRepository;

    public ConfirmBookingCommandHandler(IRepository<Booking> bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<Result> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId,cancellationToken);
        if (booking == null)
            return Result.Failure(BookingErrors.BookingNotFound());

        if(request.UserId!=booking.UserId)
            return Result.Failure(BookingErrors.UserNotOwnerOfBooking());
        
        if(booking.Status == BookingStatus.Pending)
            booking.Status = BookingStatus.Confirmed;
        else return Result.Failure(BookingErrors.BookingCannotBeConfirmed());
        
        await _bookingRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}