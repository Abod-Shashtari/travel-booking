using MediatR;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Bookings.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Bookings.CompleteBooking;

public class CompleteBookingCommandHandler:IRequestHandler<CompleteBookingCommand,Result>
{
    private readonly IRepository<Booking> _bookingRepository;

    public CompleteBookingCommandHandler(IRepository<Booking> bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<Result> Handle(CompleteBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId,cancellationToken);
        if (booking == null)
            return Result.Failure(BookingErrors.BookingNotFound());

        booking.Status = BookingStatus.Completed;
        
        await _bookingRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}