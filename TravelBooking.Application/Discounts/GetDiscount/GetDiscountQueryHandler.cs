using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Discounts.Errors;

namespace TravelBooking.Application.Discounts.GetDiscount;

public class GetDiscountQueryHandler:IRequestHandler<GetDiscountQuery,Result<DiscountResponse?>>
{
    private readonly IRepository<Discount> _discountRepository;
    private readonly IMapper _mapper;
    
    public GetDiscountQueryHandler(IRepository<Discount> discountRepository, IMapper mapper)
    {
        _discountRepository = discountRepository;
        _mapper = mapper;
    }
    
    public async Task<Result<DiscountResponse?>> Handle(GetDiscountQuery request, CancellationToken cancellationToken)
    {
        var discount = await _discountRepository.GetByIdAsync(request.DiscountId, cancellationToken);
        if (discount == null) return Result<DiscountResponse?>.Failure(DiscountErrors.DiscountNotFound());
        var discountResponse = _mapper.Map<DiscountResponse>(discount);
        return Result<DiscountResponse?>.Success(discountResponse);
    }
}