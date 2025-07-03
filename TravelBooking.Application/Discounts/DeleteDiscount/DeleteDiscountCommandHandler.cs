using MediatR;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Discounts.Errors;

namespace TravelBooking.Application.Discounts.DeleteDiscount;

public class DeleteDiscountCommandHandler:IRequestHandler<DeleteDiscountCommand,Result>
{
    private readonly IRepository<Discount> _discountRepository;
    public DeleteDiscountCommandHandler(IRepository<Discount> discountRepository)
    {
        _discountRepository = discountRepository;
    }

    public async Task<Result> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
    {
        var discount= await _discountRepository.GetByIdAsync(request.DiscountId,cancellationToken);
        if (discount == null) return Result.Failure(DiscountErrors.DiscountNotFound());
        
        _discountRepository.Delete(discount);
        await _discountRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}