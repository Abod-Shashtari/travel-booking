using AutoFixture;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Authentication.Entities;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.UserActivity.Entites;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Infrastructure;

namespace Infrastructure.Tests;

public static class TestDataFactory
{
    public static City CreateCity(Fixture fixture)
    {
        return fixture.Build<City>()
            .Without(c => c.Hotels)
            .Without(c => c.ThumbnailImage)
            .Without(c => c.ThumbnailImageId)
            .Create();
    }

    public static User CreateUser(Fixture fixture)
    {
        return fixture.Build<User>()
            .Without(c => c.HotelVisits)
            .Without(c => c.Reviews)
            .Without(c => c.Bookings)
            .Create();
    }

    public static Hotel CreateHotel(TravelBookingDbContext context, Fixture fixture)
    {
        var city = CreateCity(fixture);
        context.Cities.Add(city);
        context.SaveChanges();
        
        var owner =  CreateUser(fixture);
        context.Users.Add(owner);
        context.SaveChanges();

        return fixture.Build<Hotel>()
            .Without(c => c.RoomTypes)
            .Without(c => c.Bookings)
            .Without(c => c.HotelVisits)
            .Without(c => c.Reviews)
            .Without(c => c.ThumbnailImage)
            .Without(c => c.ThumbnailImageId)
            .Without(c => c.City)
            .Without(c => c.Owner)
            .With(c => c.CityId,city.Id)
            .With(c => c.OwnerId,owner.Id)
            .Create();
    }
    
    public static Amenity CreateAmenity(Fixture fixture)
    {
        return fixture.Build<Amenity>()
            .Without(c => c.RoomsTypes)
            .Create();
    }
    
    public static Review CreateReview(TravelBookingDbContext context, Fixture fixture)
    {
        var hotel = CreateHotel(context,fixture);
        context.Hotels.Add(hotel);
        context.SaveChanges();
        
        var user =  CreateUser(fixture);
        context.Users.Add(user);
        context.SaveChanges();
        
        return fixture.Build<Review>()
            .With(c => c.UserId, user.Id)
            .With(c => c.HotelId, hotel.Id)
            .Without(c => c.Hotel)
            .Without(c => c.User)
            .Create();
    }
    
    public static HotelVisit CreateHotelVisit(TravelBookingDbContext context, Fixture fixture)
    {
        var hotel = CreateHotel(context,fixture);
        context.Hotels.Add(hotel);
        context.SaveChanges();
        
        var user =  CreateUser(fixture);
        context.Users.Add(user);
        context.SaveChanges();
        
        return fixture.Build<HotelVisit>()
            .With(c => c.HotelId, hotel.Id)
            .With(c => c.UserId, user.Id)
            .Without(c => c.User)
            .Without(c => c.Hotel)
            .Create();
    }
    public static RoomType CreateRoomType(TravelBookingDbContext context, Fixture fixture)
    {
        var hotel = CreateHotel(context,fixture);
        context.Hotels.Add(hotel);
        context.SaveChanges();
        
        return fixture.Build<RoomType>()
            .Without(c => c.Discounts)
            .Without(c => c.Rooms)
            .Without(c => c.Amenities)
            .Without(c => c.Hotel)
            .With(c => c.HotelId,hotel.Id)
            .Create();
    }
    
    public static Room CreateRoom(TravelBookingDbContext context, Fixture fixture)
    {
        var roomType = CreateRoomType(context,fixture);
        context.RoomsTypes.Add(roomType);
        context.SaveChanges();
        
        return fixture.Build<Room>()
            .Without(c => c.Bookings)
            .Without(c => c.RoomType)
            .With(c => c.RoomTypeId,roomType.Id)
            .Create();
    }
    
    public static Discount CreateDiscount(TravelBookingDbContext context, Fixture fixture)
    {
        var roomType = CreateRoomType(context,fixture);
        context.RoomsTypes.Add(roomType);
        context.SaveChanges();
        
        return fixture.Build<Discount>()
            .With(x=>x.RoomTypeId,roomType.Id)
            .Without(x=>x.RoomType)
            .Create();
    }
    
    public static Booking CreateBooking(TravelBookingDbContext context, Fixture fixture)
    {
        var hotel = CreateHotel(context,fixture);
        context.Hotels.Add(hotel);
        context.SaveChanges();
        
        var user =  CreateUser(fixture);
        context.Users.Add(user);
        context.SaveChanges();
        
        var room = CreateRoom(context,fixture);
        context.Rooms.Add(room);
        context.SaveChanges();

        return fixture.Build<Booking>()
            .With(c => c.HotelId,hotel.Id)
            .With(c => c.UserId,user.Id)
            .With(x=>x.Rooms,[room])
            .With(x=>x.Status,BookingStatus.Pending)
            .Without(x=>x.Hotel)
            .Without(x=>x.User)
            .Create();
    }
    
    public static Image CreateImage(TravelBookingDbContext context, Fixture fixture)
    {
        var hotel = CreateHotel(context,fixture);
        context.Hotels.Add(hotel);
        context.SaveChanges();
        
        return fixture.Build<Image>()
            .With(i=>i.EntityType,EntityType.Hotels)
            .With(i=>i.EntityId,hotel.Id)
            .Create();
    }
    
    public static TokenWhiteList CreateTokenWhiteList(Fixture fixture)
    {
        return fixture.Build<TokenWhiteList>()
            .Create();
    }
    
    public static TokenWhiteList CreateTokenWhiteListWithUserId(Fixture fixture,Guid userId)
    {
        return fixture.Build<TokenWhiteList>()
            .With(t=>t.UserId,userId)
            .Create();
    }
}