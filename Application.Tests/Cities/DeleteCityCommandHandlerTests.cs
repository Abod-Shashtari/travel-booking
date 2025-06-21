using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Cities.DeleteCity;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Cities.Errors;
using TravelBooking.Domain.Common.Interfaces;

namespace Application.Tests.Cities;

public class DeleteCityCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<City>> _cityRepository;
    private readonly DeleteCityCommandHandler _handler;

    public DeleteCityCommandHandlerTests()
    {
        _fixture   = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _cityRepository  = new Mock<IRepository<City>>();
        _handler   = new DeleteCityCommandHandler(_cityRepository.Object);
    }

    [Fact]
    public async Task DeleteCityCommandHandler_ExistingCity_ShouldDeleteAndReturnSuccess()
    {
        // Arrange
        var cityId = _fixture.Create<Guid>();
        var command    = new DeleteCityCommand(cityId);
        var city   = _fixture.Create<City>();

        _cityRepository
            .Setup(r => r.GetByIdAsync(cityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(city);
        _cityRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _cityRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteCityCommandHandler_NonExistingCity_ShouldReturnFailure()
    {
        // Arrange
        var cityId = _fixture.Create<Guid>();
        var command    = new DeleteCityCommand(cityId);

        _cityRepository
            .Setup(r => r.GetByIdAsync(cityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((City)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CityErrors.CityNotFound());
        _cityRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}