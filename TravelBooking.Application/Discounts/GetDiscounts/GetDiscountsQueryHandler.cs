using AutoMapper;
using MediatR;
using TravelBooking.Application.Cities.GetCities;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Discounts.Entities;

namespace TravelBooking.Application.Discounts.GetDiscounts;

public class GetDiscountsQueryHandler:IRequestHandler<GetDiscountsQuery, Result<PaginatedList<DiscountResponse>>>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Discount> _discountRepository;

    public GetDiscountsQueryHandler(IMapper mapper, IRepository<Discount> discountRepository)
    {
        _mapper = mapper;
        _discountRepository = discountRepository;
    }

    public async Task<Result<PaginatedList<DiscountResponse>>> Handle(GetDiscountsQuery request, CancellationToken cancellationToken)
    {
        var specification = new PaginationSpecification<Discount>(request.PageNumber, request.PageSize,discount=>discount.CreatedAt,true);
        var discounts= await _discountRepository.GetPaginatedListAsync(
            specification,
            cancellationToken
        );

        var mappedItems = _mapper.Map<List<DiscountResponse>>(discounts.Data);
        var discountsResponse = new PaginatedList<DiscountResponse>(mappedItems, discounts.TotalCount, request.PageSize, request.PageNumber);
        
        return Result<PaginatedList<DiscountResponse>>.Success(discountsResponse);
    }
}