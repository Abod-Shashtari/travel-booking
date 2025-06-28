using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Payment.CreatePaymentIntent;

public record CreatePaymentIntentCommand(Guid UserId, Guid BookingId):IRequest<Result<PaymentIntentResponse?>>;