using AutoFixture;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Common.Entities;
using TravelBooking.Infrastructure;
using TravelBooking.Infrastructure.Repositories;

namespace Infrastructure.Tests;

public abstract class RepositoryTestsBase<TEntity> : IDisposable
    where TEntity : EntityBase
{
    protected readonly TravelBookingDbContext Context;
    protected readonly Fixture Fixture;
    private readonly Repository<TEntity> _repository;
    private readonly SqliteConnection _connection;

    protected RepositoryTestsBase(Func<TravelBookingDbContext, Repository<TEntity>> repository)
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        
        var options = new DbContextOptionsBuilder<TravelBookingDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new TravelBookingDbContext(options);
        Context.Database.EnsureCreated();
        _repository = repository(Context);

        Fixture = new Fixture();
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Close();
    }

    protected abstract TEntity CreateEntity();
    protected abstract DbSet<TEntity> DbSet { get; }

    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        var entity = CreateEntity();
        var result = await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        result.Should().Be(entity.Id);
        var added = await DbSet.FindAsync(entity.Id);
        added.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_ShouldRemoveEntity()
    {
        var entity = CreateEntity();
        await DbSet.AddAsync(entity);
        await _repository.SaveChangesAsync();

        _repository.Delete(entity);
        await _repository.SaveChangesAsync();

        var deleted = await DbSet.FindAsync(entity.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity()
    {
        var entity = CreateEntity();
        await DbSet.AddAsync(entity);
        await _repository.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(entity.Id);
        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
    }

    [Fact]
    public async Task IsExistsByIdAsync_ShouldReturnTrue_WhenExists()
    {
        var entity = CreateEntity();
        await DbSet.AddAsync(entity);
        await _repository.SaveChangesAsync();

        var result = await _repository.IsExistsByIdAsync(entity.Id);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsExistsByIdAsync_ShouldReturnFalse_WhenNotExists()
    {
        var result = await _repository.IsExistsByIdAsync(Guid.NewGuid());
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task IsExists_ShouldReturnTrue_WhenExists()
    {
        var entity = CreateEntity();
        if (entity is Booking) return;
        await DbSet.AddAsync(entity);
        await _repository.SaveChangesAsync();
        
        var result = await _repository.IsExistAsync(entity);
        
        result.Should().BeTrue();
    }
}
