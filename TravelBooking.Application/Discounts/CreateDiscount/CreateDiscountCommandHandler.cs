using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Discounts.Errors;

namespace TravelBooking.Application.Discounts.CreateDiscount;

public class CreateDiscountCommandHandler:IRequestHandler<CreateDiscountCommand,Result<DiscountResponse?>>
{
    private readonly IRepository<Discount> _discountRepository;
    private readonly IMapper _mapper;
    
    public CreateDiscountCommandHandler(IRepository<Discount> discountRepository, IMapper mapper)
    {
        _discountRepository = discountRepository;
        _mapper = mapper;
    }

    public async Task<Result<DiscountResponse?>> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
    {
        var discount = _mapper.Map<Discount>(request);
        if (await _discountRepository.IsExistAsync(discount, cancellationToken))
            return Result<DiscountResponse>.Failure(DiscountErrors.DiscountAlreadyExists());
        
        await _discountRepository.AddAsync(discount,cancellationToken);
        await _discountRepository.SaveChangesAsync(cancellationToken);
        var discountResponse = _mapper.Map<DiscountResponse>(discount);
        return Result<DiscountResponse?>.Success(discountResponse);
    }
}