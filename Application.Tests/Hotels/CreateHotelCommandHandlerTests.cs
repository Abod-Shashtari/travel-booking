using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Hotels.CreateHotel;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Cities.Errors;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.Users.Errors;
using TravelBooking.Domain.Users.Interfaces;

namespace Application.Tests.Hotels;

public class CreateHotelCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Hotel>> _hotelRepository;
    private readonly Mock<IRepository<City>> _cityRepository;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly CreateHotelCommandHandler _handler;

    public CreateHotelCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _hotelRepository = new Mock<IRepository<Hotel>>();
        _cityRepository = new Mock<IRepository<City>>();
        _userRepository = new Mock<IUserRepository>();
        _mapper = new Mock<IMapper>();

        _handler = new CreateHotelCommandHandler(
            _hotelRepository.Object,
            _mapper.Object,
            _cityRepository.Object,
            _userRepository.Object
        );
    }

    [Fact]
    public async Task CreateHotelCommandHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<CreateHotelCommand>();
        var hotel = _fixture.Create<Hotel>();
        var expectedId = _fixture.Create<Guid>();
        var hotelResponse = _fixture.Create<HotelResponse>();

        _cityRepository.Setup(r => r.IsExistsByIdAsync(command.CityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _userRepository.Setup(r => r.IsExistsByIdAsync(command.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mapper.Setup(m => m.Map<Hotel>(command))
            .Returns(hotel);
        _hotelRepository.Setup(r => r.IsExistAsync(hotel, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _hotelRepository.Setup(r => r.AddAsync(hotel, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);
        _hotelRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mapper.Setup(m => m.Map<HotelResponse>(It.IsAny<Hotel>()))
            .Returns(hotelResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(hotelResponse);
        _hotelRepository.Verify(r => r.AddAsync(It.IsAny<Hotel>(), It.IsAny<CancellationToken>()), Times.Once);
        _hotelRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateHotelCommandHandler_CityDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<CreateHotelCommand>();

        _cityRepository.Setup(r => r.IsExistsByIdAsync(command.CityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CityErrors.CityNotFound());
        _hotelRepository.Verify(r => r.AddAsync(It.IsAny<Hotel>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateHotelCommandHandler_OwnerDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<CreateHotelCommand>();

        _cityRepository.Setup(r => r.IsExistsByIdAsync(command.CityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _userRepository.Setup(r => r.IsExistsByIdAsync(command.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.UserNotFound());
        _hotelRepository.Verify(r => r.AddAsync(It.IsAny<Hotel>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateHotelCommandHandler_HotelAlreadyExists_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<CreateHotelCommand>();
        var hotel = _fixture.Create<Hotel>();

        _cityRepository.Setup(r => r.IsExistsByIdAsync(command.CityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _userRepository.Setup(r => r.IsExistsByIdAsync(command.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mapper.Setup(m => m.Map<Hotel>(command))
            .Returns(hotel);
        _hotelRepository.Setup(r => r.IsExistAsync(It.IsAny<Hotel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(HotelErrors.HotelAlreadyExists());
        _hotelRepository.Verify(r => r.AddAsync(It.IsAny<Hotel>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}