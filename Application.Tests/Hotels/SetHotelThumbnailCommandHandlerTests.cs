using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Application.Hotels.SetThumbnail;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.Images.Errors;

namespace Application.Tests.Hotels;

public class SetHotelThumbnailCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Hotel>> _hotelRepository;
    private readonly Mock<IImageRepository> _imageRepository;
    private readonly SetHotelThumbnailCommandHandler _handler;

    public SetHotelThumbnailCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _hotelRepository = new Mock<IRepository<Hotel>>();
        _imageRepository = new Mock<IImageRepository>();

        _handler = new SetHotelThumbnailCommandHandler(
            _hotelRepository.Object,
            _imageRepository.Object
        );
    }

    [Fact]
    public async Task SetHotelThumbnailCommandHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<SetHotelThumbnailCommand>();
        var hotel = _fixture.Create<Hotel>();

        _imageRepository.Setup(r => r.IsExistsByIdAsync(command.ImageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _hotelRepository.Setup(r => r.GetByIdAsync(command.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotel);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        hotel.ThumbnailImageId.Should().Be(command.ImageId);
    }

    [Fact]
    public async Task SetHotelThumbnailCommandHandler_ImageDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<SetHotelThumbnailCommand>();

        _imageRepository.Setup(r => r.IsExistsByIdAsync(command.ImageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ImageErrors.ImageNotFound());
        _hotelRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SetHotelThumbnailCommandHandler_HotelDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<SetHotelThumbnailCommand>();

        _imageRepository.Setup(r => r.IsExistsByIdAsync(command.ImageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _hotelRepository.Setup(r => r.GetByIdAsync(command.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Hotel?)null);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(HotelErrors.HotelNotFound());
    }
}