using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Entities;

namespace TravelBooking.Domain;

public class TravelBookingDbContext:DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<TokenWhiteList> TokenWhiteList { get; set; }
    public TravelBookingDbContext(DbContextOptions<TravelBookingDbContext> options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}