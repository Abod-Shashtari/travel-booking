using System.Linq.Expressions;
using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Hotels.GetHotel;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;

namespace Application.Tests.Hotels;

public class GetHotelQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Hotel>> _hotelRepository;
    private readonly GetHotelQueryHandler _handler;

    public GetHotelQueryHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        _hotelRepository = new Mock<IRepository<Hotel>>();
        
        _handler = new GetHotelQueryHandler(_hotelRepository.Object);
    }

    [Fact]
    public async Task GetHotelQueryHandler_HotelExists_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<GetHotelQuery>();
        var hotelProjection = _fixture.Create<HotelResponse>();

        _hotelRepository
            .Setup(r => r.GetByIdAsync(
                command.HotelId,
                It.IsAny<Expression<Func<Hotel, HotelResponse>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotelProjection);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(hotelProjection);
    }

    [Fact]
    public async Task GetHotelQueryHandler_HotelDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<GetHotelQuery>();

        _hotelRepository
            .Setup(r => r.GetByIdAsync(
                command.HotelId,
                It.IsAny<Expression<Func<Hotel, HotelResponse>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((HotelResponse?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(HotelErrors.HotelNotFound());
    }
}