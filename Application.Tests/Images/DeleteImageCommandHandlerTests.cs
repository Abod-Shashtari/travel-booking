using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Application.Images.DeleteImage;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Images.Errors;

namespace Application.Tests.Images;

public class DeleteImageCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IImageService> _imageService;
    private readonly Mock<IRepository<Image>> _imageRepository;
    private readonly DeleteImageCommandHandler _handler;

    public DeleteImageCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _imageService = new Mock<IImageService>();
        _imageRepository = new Mock<IRepository<Image>>();
        _handler = new DeleteImageCommandHandler(_imageService.Object, _imageRepository.Object);
    }

    [Fact]
    public async Task DeleteImageCommandHandler_ImageNotFound_ShouldReturnFailure()
    {
        // Arrange
        var imageGuid= _fixture.Create<Guid>();
        var command = new DeleteImageCommand(imageGuid);
        
        _imageRepository.Setup(r => r.GetByIdAsync(command.ImageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Image)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ImageErrors.ImageNotFound());
        _imageService.Verify(s => s.DeleteImageAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeleteImageCommandHandler_DeleteFails_ShouldReturnFailure()
    {
        // Arrange
        var imageGuid= _fixture.Create<Guid>();
        var command = new DeleteImageCommand(imageGuid);
        var image = _fixture.Create<Image>();
        _imageRepository.Setup(r => r.GetByIdAsync(command.ImageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(image);
        _imageService.Setup(s => s.DeleteImageAsync(image.Url))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ImageErrors.ErrorWhileDeleting());
        _imageRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteImageCommandHandler_ValidRequest_ShouldDeleteAndReturnSuccess()
    {
        // Arrange
        var imageGuid= _fixture.Create<Guid>();
        var command = new DeleteImageCommand(imageGuid);
        var image = _fixture.Create<Image>();
        _imageRepository.Setup(r => r.GetByIdAsync(command.ImageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(image);
        _imageService.Setup(s => s.DeleteImageAsync(image.Url))
            .ReturnsAsync(true);
        _imageRepository.Setup(r => r.Delete(image));
        _imageRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _imageService.Verify(s => s.DeleteImageAsync(image.Url), Times.Once);
        _imageRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}