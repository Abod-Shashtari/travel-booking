using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Bookings.GetBookings;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace Application.Tests.Bookings;

public class GetBookingsQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Booking>> _bookingRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly GetBookingsQueryHandler _handler;

    public GetBookingsQueryHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _bookingRepository = new Mock<IRepository<Booking>>();
        _mapper = new Mock<IMapper>();
        _handler = new GetBookingsQueryHandler(_bookingRepository.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetBookingsQueryHandler_ValidRequest_ShouldReturnPaginatedBookingResponse()
    {
        // Arrange
        var query = _fixture.Create<GetBookingsQuery>();
        var bookings = _fixture.CreateMany<Booking>(5).ToList();
        var mappedBookingResponses = _fixture.CreateMany<BookingResponse>(5).ToList();

        var paginatedBookings = new PaginatedList<Booking>(
            bookings,
            bookings.Count,
            query.PageSize,
            query.PageNumber
        );

        _bookingRepository.Setup(r => r.GetPaginatedListAsync(
                It.IsAny<GetBookingsOfUserSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedBookings);

        _mapper.Setup(m => m.Map<List<BookingResponse>>(bookings))
            .Returns(mappedBookingResponses);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Data.Should().BeEquivalentTo(mappedBookingResponses);
        result.Value.TotalCount.Should().Be(paginatedBookings.TotalCount);
        result.Value.CurrentPage.Should().Be(paginatedBookings.CurrentPage);
        result.Value.PageSize.Should().Be(paginatedBookings.PageSize);
    }
}