using MediatR;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Bookings.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Users.Interfaces;

namespace TravelBooking.Application.Payment.CreatePaymentIntent;

public class CreatePaymentIntentCommandHandler:IRequestHandler<CreatePaymentIntentCommand,Result<PaymentIntentResponse?>>
{
    private readonly IPaymentService _paymentService;
    private readonly IRepository<Booking> _bookingRepository;
    private readonly IUserRepository _userRepository;

    public CreatePaymentIntentCommandHandler(IPaymentService paymentService, IRepository<Booking> bookingRepository, IUserRepository userRepository)
    {
        _paymentService = paymentService;
        _bookingRepository = bookingRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<PaymentIntentResponse?>> Handle(CreatePaymentIntentCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId,cancellationToken);
        if(booking == null) return Result<PaymentIntentResponse?>.Failure(BookingErrors.BookingNotFound());
        
        if(booking.UserId!=request.UserId) return Result<PaymentIntentResponse?>.Failure(BookingErrors.UserNotOwnerOfBooking());
        
        if(booking.Status != BookingStatus.Pending) return Result<PaymentIntentResponse?>.Failure(BookingErrors.BookingCannotBeConfirmed());
        
        var user=await _userRepository.GetByIdAsync(request.UserId,cancellationToken);
        var userEmail=user!.Email;
        
        var clientSecret= await _paymentService.CreatePaymentIntentAsync(
            booking.TotalCost,
            "usd",
            userEmail,
            booking.Id.ToString(),
            cancellationToken
        );
        
        return Result<PaymentIntentResponse?>.Success(new PaymentIntentResponse(clientSecret));
    }
}