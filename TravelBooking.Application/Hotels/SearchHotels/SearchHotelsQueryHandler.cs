﻿using System.Linq.Expressions;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Hotels.Specifications;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Hotels.SearchHotels;

public class SearchHotelsQueryHandler:IRequestHandler<SearchHotelsQuery, Result<PaginatedList<HotelResponse>>>
{
    private readonly IRepository<Hotel> _hotelRepository;
    public SearchHotelsQueryHandler(IRepository<Hotel> hotelRepository)
    {
        _hotelRepository = hotelRepository;
    }

    public async Task<Result<PaginatedList<HotelResponse>>> Handle(SearchHotelsQuery request, CancellationToken cancellationToken)
    {
        var spec = new HotelSearchSpecification(request.HotelFilter, request.PageNumber, request.PageSize);
        
        Expression<Func<Hotel, HotelResponse>> selector = hotel => new HotelResponse(
            hotel.Id,
            hotel.Name,
            hotel.Location,
            hotel.Description,
            hotel.City!.Name,
            hotel.CityId,
            hotel.OwnerId,
            hotel.StarRating,
            hotel.CreatedAt,
            hotel.ModifiedAt,
            new ImageResponse(
                hotel.ThumbnailImageId,
                hotel.ThumbnailImage != null ? hotel.ThumbnailImage.Url : ""
            )
        );
        
        var hotels = await _hotelRepository.GetPaginatedListAsync(
            spec,
            selector,
            cancellationToken
        );
        
        return Result<PaginatedList<HotelResponse>>.Success(hotels);
    }
}