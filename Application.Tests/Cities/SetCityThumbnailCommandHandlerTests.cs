using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Cities.SetThumbnail;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Cities.Errors;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Images.Errors;

namespace Application.Tests.Cities;

public class SetCityThumbnailCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<City>> _cityRepository;
    private readonly Mock<IRepository<Image>> _imageRepository;
    private readonly SetCityThumbnailCommandHandler _handler;

    public SetCityThumbnailCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Customize<City>(c => c
            .Without(city => city.Hotels)
        );
        _cityRepository = new Mock<IRepository<City>>();
        _imageRepository = new Mock<IRepository<Image>>();
        _handler = new SetCityThumbnailCommandHandler(
            _cityRepository.Object,
            _imageRepository.Object
        );
    }

    [Fact]
    public async Task SetCityThumbnailCommandHandle_ImageDoesNotExist_ShouldReturnImageNotFound()
    {
        // Arrange
        var command = _fixture.Create<SetCityThumbnailCommand>();
        _imageRepository
            .Setup(r => r.IsExistsByIdAsync(command.ImageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ImageErrors.ImageNotFound());

        _imageRepository.Verify(r => r.IsExistsByIdAsync(command.ImageId, It.IsAny<CancellationToken>()), Times.Once);
        _cityRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SetCityThumbnailCommandHandle_CityDoesNotExist_ShouldReturnCityNotFound()
    {
        // Arrange
        var command = _fixture.Create<SetCityThumbnailCommand>();
        _imageRepository
            .Setup(r => r.IsExistsByIdAsync(command.ImageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _cityRepository
            .Setup(r => r.GetByIdAsync(command.CityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((City)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CityErrors.CityNotFound());

        _imageRepository.Verify(r => r.IsExistsByIdAsync(command.ImageId, It.IsAny<CancellationToken>()), Times.Once);
        _cityRepository.Verify(r => r.GetByIdAsync(command.CityId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetCityThumbnailCommandHandle_ValidRequest_ShouldSetThumbnailAndReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<SetCityThumbnailCommand>();
        var city = _fixture.Create<City>();

        _imageRepository
            .Setup(r => r.IsExistsByIdAsync(command.ImageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _cityRepository
            .Setup(r => r.GetByIdAsync(command.CityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(city);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        city.ThumbnailImageId.Should().Be(command.ImageId);

        _imageRepository.Verify(r => r.IsExistsByIdAsync(command.ImageId, It.IsAny<CancellationToken>()), Times.Once);
        _cityRepository.Verify(r => r.GetByIdAsync(command.CityId, It.IsAny<CancellationToken>()), Times.Once);
    }
}