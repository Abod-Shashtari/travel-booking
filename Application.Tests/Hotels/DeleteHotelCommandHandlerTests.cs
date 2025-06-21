using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Hotels.DeleteHotel;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;

namespace Application.Tests.Hotels;

public class DeleteHotelCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Hotel>> _hotelRepository;
    private readonly DeleteHotelCommandHandler _handler;

    public DeleteHotelCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _hotelRepository = new Mock<IRepository<Hotel>>();

        _handler = new DeleteHotelCommandHandler(_hotelRepository.Object);
    }

    [Fact]
    public async Task DeleteHotelCommandHandler_HotelExists_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<DeleteHotelCommand>();
        var hotel = _fixture.Create<Hotel>();

        _hotelRepository.Setup(r => r.GetByIdAsync(command.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotel);
        _hotelRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _hotelRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteHotelCommandHandler_HotelDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<DeleteHotelCommand>();

        _hotelRepository.Setup(r => r.GetByIdAsync(command.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Hotel?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(HotelErrors.HotelNotFound());
        _hotelRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}