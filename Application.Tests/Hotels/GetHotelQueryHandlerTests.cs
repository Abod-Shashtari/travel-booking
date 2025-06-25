using System.Linq.Expressions;
using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Hotels.GetHotel;
using TravelBooking.Application.Hotels.Specifications;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.UserActivity.Entites;

namespace Application.Tests.Hotels;

public class GetHotelQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Hotel>> _hotelRepository;
    private readonly Mock<IRepository<HotelVisit>> _hotelVisitRepository;
    private readonly GetHotelQueryHandler _handler;

    public GetHotelQueryHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        _hotelRepository = new Mock<IRepository<Hotel>>();
        _hotelVisitRepository= new Mock<IRepository<HotelVisit>>();
        
        _handler = new GetHotelQueryHandler(_hotelRepository.Object,_hotelVisitRepository.Object);
    }

    [Fact]
    public async Task GetHotelQueryHandler_HotelExists_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<GetHotelQuery>();
        var hotelProjection = _fixture.Create<HotelResponse>();

        _hotelRepository.Setup(r => r.GetByIdAsync(
                command.HotelId,
                It.IsAny<Expression<Func<Hotel, HotelResponse>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotelProjection);

        var existingHotelVisitList = _fixture.CreateMany<HotelVisit>(2).ToList();
        var existedHotelVisitRecord = new PaginatedList<HotelVisit>(
            existingHotelVisitList,
            existingHotelVisitList.Count,
            existingHotelVisitList.Count,
            1
        );
        
        _hotelVisitRepository.Setup(r => r.GetPaginatedListAsync(
            It.IsAny<GetHotelVisitedSpecification>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(existedHotelVisitRecord);
        
        _hotelVisitRepository.Setup(r => r.Delete(It.IsAny<HotelVisit>()));

        _hotelVisitRepository.Setup(r => r.AddAsync(
            It.IsAny<HotelVisit>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(It.IsAny<Guid>());
        
        _hotelVisitRepository.Setup(r => r.SaveChangesAsync(
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(hotelProjection);
        
        _hotelVisitRepository.Verify(r => r.Delete(It.IsAny<HotelVisit>()), 
            Times.Exactly(existingHotelVisitList.Count));
    }

    [Fact]
    public async Task GetHotelQueryHandler_HotelDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<GetHotelQuery>();

        _hotelRepository
            .Setup(r => r.GetByIdAsync(
                command.HotelId,
                It.IsAny<Expression<Func<Hotel, HotelResponse>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((HotelResponse?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(HotelErrors.HotelNotFound());
    }
}