using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Authentication.Entities;
using TravelBooking.Domain.Common.Entities;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Infrastructure;

public class TravelBookingDbContext:DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<TokenWhiteList> TokenWhiteList { get; set; }
    public TravelBookingDbContext(DbContextOptions<TravelBookingDbContext> options):base(options)
    {
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