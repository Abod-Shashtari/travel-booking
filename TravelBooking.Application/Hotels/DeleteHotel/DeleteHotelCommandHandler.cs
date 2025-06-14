using MediatR;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;

namespace TravelBooking.Application.Hotels.DeleteHotel;

public class DeleteHotelCommandHandler : IRequestHandler<DeleteHotelCommand, Result>
{
    private readonly IRepository<Hotel> _hotelRepository;

    public DeleteHotelCommandHandler(IRepository<Hotel> hotelRepository)
    {
        _hotelRepository = hotelRepository;
    }

    public async Task<Result> Handle(DeleteHotelCommand request, CancellationToken cancellationToken)
    {
        var hotel= await _hotelRepository.GetByIdAsync(request.HotelId,cancellationToken);
        if (hotel == null) return Result.Failure(HotelErrors.HotelNotFound());
        _hotelRepository.Delete(hotel);
        await _hotelRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}