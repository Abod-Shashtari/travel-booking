using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Infrastructure.Repositories;

namespace Infrastructure.Tests;

public class ImageRepositoryTests : RepositoryTestsBase<Image>
{
    private readonly ImageRepository _imageRepository;
    
    public ImageRepositoryTests() : base(ctx => new ImageRepository(ctx))
    {
        _imageRepository = new ImageRepository(Context);
    }

    protected override Image CreateEntity()
    {
        return TestDataFactory.CreateImage(Context,Fixture);
    }
    
    protected override DbSet<Image> DbSet => Context.Images;
    
    [Fact]
    public async Task IsImageUsedAsThumbnailsAsync_ImageIsNotUsedAsThumbnail_ShouldReturnNull()
    {
        // Arrange
        var image = CreateEntity();
        await DbSet.AddAsync(image);
        await Context.SaveChangesAsync();

        // Act
        var result = await _imageRepository.IsImageUsedAsThumbnailsAsync(image.Id);

        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task IsImageUsedAsThumbnailsAsync_ImageIsUsedAsThumbnail_ShouldReturnImageUsageInfo()
    {
        // Arrange
        var image = CreateEntity();
        await DbSet.AddAsync(image);
        await Context.SaveChangesAsync();
        
        var hotel = TestDataFactory.CreateHotel(Context, Fixture);
        hotel.ThumbnailImageId = image.Id;
        await Context.Hotels.AddAsync(hotel);
        await Context.SaveChangesAsync();

        // Act
        var result = await _imageRepository.IsImageUsedAsThumbnailsAsync(image.Id);

        // Assert
        result.Should().NotBeNull();
        result.EntityType.Should().Be("Hotels");
        result.EntityId.Should().Be(hotel.Id);
    }
}