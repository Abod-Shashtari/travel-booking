using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Hotels.UpdateHotel;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Cities.Errors;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.Users.Errors;
using TravelBooking.Domain.Users.Interfaces;

namespace Application.Tests.Hotels;

public class UpdateHotelCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Hotel>> _hotelRepository;
    private readonly Mock<IRepository<City>> _cityRepository;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly UpdateHotelCommandHandler _handler;

    public UpdateHotelCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _hotelRepository = new Mock<IRepository<Hotel>>();
        _cityRepository = new Mock<IRepository<City>>();
        _userRepository = new Mock<IUserRepository>();
        var mapper = new Mock<IMapper>();

        _handler = new UpdateHotelCommandHandler(
            _hotelRepository.Object,
            mapper.Object,
            _userRepository.Object,
            _cityRepository.Object
        );
    }

    [Fact]
    public async Task UpdateHotelCommandHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<UpdateHotelCommand>();
        var hotel = _fixture.Create<Hotel>();

        _cityRepository.Setup(r => r.IsExistsByIdAsync(command.CityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _userRepository.Setup(r => r.IsExistsByIdAsync(command.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _hotelRepository.Setup(r => r.GetByIdAsync(command.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotel);
        _hotelRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _hotelRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateHotelCommandHandler_CityDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<UpdateHotelCommand>();

        _cityRepository.Setup(r => r.IsExistsByIdAsync(command.CityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CityErrors.CityNotFound());
        _hotelRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateHotelCommandHandler_OwnerDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<UpdateHotelCommand>();

        _cityRepository.Setup(r => r.IsExistsByIdAsync(command.CityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _userRepository.Setup(r => r.IsExistsByIdAsync(command.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.UserNotFound());
        _hotelRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateHotelCommandHandler_HotelDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<UpdateHotelCommand>();

        _cityRepository.Setup(r => r.IsExistsByIdAsync(command.CityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _userRepository.Setup(r => r.IsExistsByIdAsync(command.OwnerId, It.IsAny<CancellationToken>()))
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