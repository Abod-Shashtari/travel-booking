using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Application.Images.UploadImage;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Images.Errors;
using TravelBooking.Domain.Images.Interfaces;

namespace Application.Tests.Images;

public class UploadImageCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IImageService> _imageService;
    private readonly Mock<IImageRepository> _imageRepository;
    private readonly UploadImageCommandHandler _handler;

    public UploadImageCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization());

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _imageService = new Mock<IImageService>();
        _imageRepository = new Mock<IImageRepository>();
        var mapper = new Mock<IMapper>();
        _handler = new UploadImageCommandHandler(_imageService.Object, _imageRepository.Object, mapper.Object);
    }

    [Fact]
    public async Task UploadImageCommandHandler_InvalidEntityType_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Build<UploadImageCommand>()
            .With(c => c.EntityName, "Invalid")
            .Create();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ImageErrors.ImageInvalidEntityType());
        _imageService.Verify(s => s.AddImageAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()), Times.Never);
        _imageRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task UploadImageCommandHandler_ImageAlreadyExists_ShouldReturnFailureAndDeleteUploaded()
    {
        // Arrange
        var validType = Enum.GetValues<EntityType>().First().ToString();
        var command = _fixture.Build<UploadImageCommand>()
            .With(c => c.EntityName, validType)
            .Create();
    
        var imageUrl = _fixture.Create<string>();
    
        _imageService.Setup(s => s.AddImageAsync(command.Image, It.IsAny<CancellationToken>()))
            .ReturnsAsync(imageUrl);
    
        _imageRepository.Setup(r => r.IsExistAsync(It.IsAny<Image>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
    
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ImageErrors.ImageAlreadyExists());
    
        _imageService.Verify(s => s.AddImageAsync(command.Image, It.IsAny<CancellationToken>()), Times.Once);
        _imageService.Verify(s => s.DeleteImageAsync(imageUrl), Times.Once);
        _imageRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task UploadImageCommandHandler_ValidRequest_ShouldUploadAndReturnSuccess()
    {
        // Arrange
        var validType = Enum.GetValues<EntityType>().First().ToString();
        var expectedGuid= _fixture.Create<Guid>();
        var command = _fixture.Build<UploadImageCommand>()
            .With(c => c.EntityName, validType)
            .Create();
    
        var imageUrl = _fixture.Create<string>();
    
        _imageService.Setup(s => s.AddImageAsync(command.Image, It.IsAny<CancellationToken>()))
            .ReturnsAsync(imageUrl);
    
        _imageRepository.Setup(r => r.IsExistAsync(It.IsAny<Image>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
    
        _imageRepository.Setup(r => r.AddAsync(It.IsAny<Image>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGuid);
    
        _imageRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
    
        // Assert
        result.IsSuccess.Should().BeTrue();
        
        _imageService.Verify(s => s.AddImageAsync(command.Image, It.IsAny<CancellationToken>()), Times.Once);
        _imageRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}