using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Images.GetImages;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Images.Errors;

namespace Application.Tests.Images;

public class GetImagesQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Image>> _imageRepository;
    private readonly GetImagesQueryHandler _handler;

    public GetImagesQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        _imageRepository = new Mock<IRepository<Image>>();

        _handler = new GetImagesQueryHandler(_imageRepository.Object);
    }

    [Theory]
    [InlineData("hotels",true)]
    [InlineData("cities",true)]
    [InlineData("room-types",true)]
    [InlineData("discounts",false)]
    public async Task GetImagesQueryHandler_ShouldReturnSuccessWithPaginatedList(string entityName, bool isValid)
    {
        // Arrange
        var query = _fixture.Build<GetImagesQuery>()
            .With(q => q.EntityName, entityName)
            .Create();

        var images = _fixture.CreateMany<Image>(5).ToList();
        var imageResponses = images.Select(i => new ImageResponse(i.Id, i.Url)).ToList();
        var paginatedList = new PaginatedList<ImageResponse>(imageResponses, imageResponses.Count, query.PageNumber, query.PageSize);

        _imageRepository.Setup(repo => repo.GetPaginatedListAsync(
                It.IsAny<ISpecification<Image>>(),
                It.IsAny<Expression<Func<Image, ImageResponse>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedList);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        if (isValid)
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Data.Should().BeEquivalentTo(imageResponses);
            result.Value.TotalCount.Should().Be(paginatedList.TotalCount);
            result.Value.PageSize.Should().Be(paginatedList.PageSize);
            result.Value.CurrentPage.Should().Be(paginatedList.CurrentPage);
        }
        else
        {
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeEquivalentTo(ImageErrors.ImageInvalidEntityType());
        }
    }
}