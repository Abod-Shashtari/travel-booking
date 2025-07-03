using AutoMapper;
using MediatR;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Discounts.Errors;

namespace TravelBooking.Application.Discounts.UpdateDiscount;

public class UpdateDiscountCommandHandler:IRequestHandler<UpdateDiscountCommand,Result>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Discount> _discountRepository;
    
    public UpdateDiscountCommandHandler(IMapper mapper, IRepository<Discount> discountRepository)
    {
        _mapper = mapper;
        _discountRepository = discountRepository;
    }
    public async Task<Result> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
    {
        var discount = await _discountRepository.GetByIdAsync(request.DiscountId,cancellationToken);
        if(discount==null) return Result.Failure(DiscountErrors.DiscountNotFound());
        
        _mapper.Map(request,discount);
        await _discountRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}