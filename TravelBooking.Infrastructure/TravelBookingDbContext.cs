﻿using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Common.Entities;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.Tokens.Entities;
using TravelBooking.Domain.UserActivity.Entites;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Infrastructure;

public class TravelBookingDbContext:DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<TokenWhiteList> TokenWhiteList { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Amenity> Amenities { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomType> RoomsTypes { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<HotelVisit> HotelVisits { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public TravelBookingDbContext(DbContextOptions<TravelBookingDbContext> options):base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken=default)
    {
        ApplyAudit();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAudit()
    {
        var entries= ChangeTracker.Entries().Where(e=>e.Entity is AuditEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
                ((AuditEntity)entry.Entity).CreatedAt = DateTime.UtcNow;
            else if (entry.State == EntityState.Modified)
                ((AuditEntity)entry.Entity).ModifiedAt= DateTime.UtcNow;
        }
    }
}