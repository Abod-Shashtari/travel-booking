using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Cities.UpdateCity;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Cities.Errors;
using TravelBooking.Domain.Common.Interfaces;

namespace Application.Tests.Cities;

public class UpdateCityCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<City>> _cityRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly UpdateCityCommandHandler _handler;

    public UpdateCityCommandHandlerTests()
    {
        _fixture    = new Fixture();
        _fixture.Customize<City>(c => c
            .Without(city => city.Hotels)
        );
        _cityRepository   = new Mock<IRepository<City>>();
        _mapper = new Mock<IMapper>();
        _handler    = new UpdateCityCommandHandler(_mapper.Object, _cityRepository.Object);
    }

    [Fact]
    public async Task UpdateCityCommandHandler_CityNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<UpdateCityCommand>();
        _cityRepository
            .Setup(r => r.GetByIdAsync(command.CityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((City)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CityErrors.CityNotFound());
        _cityRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCityCommandHandler_CityExists_ShouldReturnSuccessAndSave()
    {
        // Arrange
        var existing = _fixture.Create<City>();
        var command = _fixture.Create<UpdateCityCommand>();

        _cityRepository
            .Setup(r => r.GetByIdAsync(command.CityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        
        _cityRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mapper.Verify(m => m.Map(command, existing), Times.Once);
        _cityRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}