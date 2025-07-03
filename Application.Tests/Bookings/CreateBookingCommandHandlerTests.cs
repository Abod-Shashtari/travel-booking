using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Bookings.CreateBooking;
using TravelBooking.Application.Bookings.Specifications;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Bookings.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.Errors;
using TravelBooking.Domain.RoomTypes.Entities;

namespace Application.Tests.Bookings;

public class CreateBookingCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Booking>> _bookingRepository;
    private readonly Mock<IRepository<Hotel>> _hotelRepository;
    private readonly Mock<IRepository<Room>> _roomRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly CreateBookingCommandHandler _handler;

    public CreateBookingCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _bookingRepository = new Mock<IRepository<Booking>>();
        _hotelRepository = new Mock<IRepository<Hotel>>();
        _roomRepository = new Mock<IRepository<Room>>();
        _mapper = new Mock<IMapper>();

        _handler = new CreateBookingCommandHandler(
            _bookingRepository.Object,
            _hotelRepository.Object,
            _roomRepository.Object,
            _mapper.Object
        );
    }

    [Fact]
    public async Task CreateBookingCommandHandler_HotelDoesNotExist_ShouldReturnHotelNotFound()
    {
        // Arrange
        var command = _fixture.Create<CreateBookingCommand>();
        _hotelRepository
            .Setup(r => r.IsExistsByIdAsync(command.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(HotelErrors.HotelNotFound());
    }

    [Fact]
    public async Task CreateBookingCommandHandler_RoomsNotFound_ShouldReturnRoomNotFound()
    {
        // Arrange
        var command = _fixture.Create<CreateBookingCommand>();
        _hotelRepository
            .Setup(r => r.IsExistsByIdAsync(command.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var pagedRooms = new PaginatedList<Room>(
            [_fixture.Create<Room>()],
            1,
            1,
            1
        );
        _roomRepository
            .Setup(r => r.GetPaginatedListAsync(
                It.IsAny<GetBookedRoomsSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedRooms);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoomErrors.RoomNotFound());
    }

    [Fact]
    public async Task CreateBookingCommandHandler_BookingConflict_ShouldReturnBookingConflict()
    {
        // Arrange
        var command = _fixture.Create<CreateBookingCommand>();
        _hotelRepository
            .Setup(r => r.IsExistsByIdAsync(command.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var roomsList = _fixture.CreateMany<Room>(command.Rooms.Count).ToList();
        var pagedRooms = new PaginatedList<Room>(
            roomsList,
            roomsList.Count,
            roomsList.Count,
            1
        );
        _roomRepository
            .Setup(r => r.GetPaginatedListAsync(
                It.IsAny<GetBookedRoomsSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedRooms);

        var booking = _fixture.Create<Booking>();
        _mapper
            .Setup(m => m.Map<Booking>(command))
            .Returns(booking);

        _bookingRepository
            .Setup(r => r.IsExistAsync(booking, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.BookingConflict());
    }

    [Fact]
    public async Task CreateBookingCommandHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var command = _fixture.Create<CreateBookingCommand>();
        
        _hotelRepository
            .Setup(r => r.IsExistsByIdAsync(command.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var roomType = _fixture.Build<RoomType>()
            .With(rt => rt.PricePerNight, 100)
            .With(rt => rt.Discounts, new List<Discount>
            {
                _fixture.Build<Discount>()
                    .With(d => d.StartDate, DateTime.Now.AddDays(-1))
                    .With(d => d.EndDate, DateTime.Now.AddDays(1))
                    .With(d => d.Percentage, 10)
                    .Create(),
            })
            .Create();

        var roomsList = command.Rooms
            .Select(_ => _fixture.Build<Room>()
                .With(r => r.RoomType, roomType)
                .Create())
            .ToList();

        var days = (command.CheckOut - command.CheckIn).Days;
        var expectedTotalCost = roomsList.Count * (roomType.PricePerNight * 0.9m) * days;

        var pagedRooms = new PaginatedList<Room>(
            roomsList,
            roomsList.Count,
            roomsList.Count,
            1
        );

        _roomRepository.Setup(r => r.GetPaginatedListAsync(
                It.IsAny<GetBookedRoomsSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedRooms);

        var booking = new Booking();
        _mapper.Setup(m => m.Map<Booking>(command))
            .Returns(booking);

        _bookingRepository.Setup(r => r.IsExistAsync(booking, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _bookingRepository.Setup(r => r.AddAsync(booking, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);
        _bookingRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var bookingResponse = _fixture.Create<BookingResponse>();
        
        _mapper
            .Setup(m => m.Map<BookingResponse>(booking))
            .Returns(bookingResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(bookingResponse with
        {
            RoomIds = command.Rooms,
            Status = nameof(BookingStatus.Pending)
        });
        booking.TotalCost.Should().Be(expectedTotalCost);
        _bookingRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}