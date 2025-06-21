using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Cities.CreateCity;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Cities.Errors;
using TravelBooking.Domain.Common.Interfaces;

namespace Application.Tests.Cities;

public class CreateCityCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<City>> _cityRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly CreateCityCommandHandler _handler;

    public CreateCityCommandHandlerTests()
    {
        _fixture     = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _cityRepository    = new Mock<IRepository<City>>();
        _mapper  = new Mock<IMapper>();
        _handler     = new CreateCityCommandHandler(_cityRepository.Object, _mapper.Object);
    }
    
    [Fact]
    public async Task CreateCityCommandHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<CreateCityCommand>();
        var expectedCityId = _fixture.Create<Guid>();
        var city = _fixture.Create<City>();

        var cityResponse   = _fixture.Create<CityResponse>();

        _mapper.Setup(m => m.Map<City>(command))
            .Returns(city);
        
        _cityRepository.Setup(r => r.IsExistAsync(city, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        _cityRepository.Setup(r => r.AddAsync(city, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCityId);
        
        _cityRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        _mapper.Setup(m => m.Map<CityResponse>(city))
            .Returns(cityResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(cityResponse);
        _cityRepository.Verify(r => r.AddAsync(It.IsAny<City>(), It.IsAny<CancellationToken>()), Times.Once);
        _cityRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateCityCommandHandler_DuplicateCity_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<CreateCityCommand>();
        var city = _fixture.Create<City>();

        _mapper
            .Setup(m => m.Map<City>(command))
            .Returns(city);
        _cityRepository
            .Setup(r => r.IsExistAsync(city, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CityErrors.CityAlreadyExists());
        _cityRepository.Verify(r => r.AddAsync(It.IsAny<City>(), It.IsAny<CancellationToken>()), Times.Never);
        _cityRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}