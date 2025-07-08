using AutoFixture;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Authentication.Entities;
using TravelBooking.Infrastructure;
using TravelBooking.Infrastructure.Repositories;

namespace Infrastructure.Tests;

public class TokenWhiteListRepositoryTests
{
    private readonly TravelBookingDbContext _context;
    private readonly Fixture _fixture;
    private readonly TokenWhiteListRepository _tokenWhiteListRepository;
    
    public TokenWhiteListRepositoryTests()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        
        var options = new DbContextOptionsBuilder<TravelBookingDbContext>()
            .UseSqlite(connection)
            .Options;

        _context = new TravelBookingDbContext(options);
        _context.Database.EnsureCreated();
        _fixture = new Fixture();
        _tokenWhiteListRepository = new TokenWhiteListRepository(_context);
    }

    private TokenWhiteList CreateEntity()
    {
        return TestDataFactory.CreateTokenWhiteList(_fixture);
    }
    
    private TokenWhiteList CreateEntityWithUserId(Guid userId)
    {
        return TestDataFactory.CreateTokenWhiteListWithUserId(_fixture, userId);
    }
    
    [Fact]
    public async Task AddAsync_ValidEntity_ShouldAddToDatabase()
    {
        // Arrange
        var entity = CreateEntity();

        // Act
        await _tokenWhiteListRepository.AddAsync(entity);
        await _tokenWhiteListRepository.SaveChangesAsync();

        // Assert
        var savedEntity = await _context.TokenWhiteList.FindAsync(entity.Id);
        savedEntity.Should().NotBeNull();
        savedEntity.Jti.Should().Be(entity.Jti);
    }

    [Fact]
    public async Task IsTokenActiveAsync_ActiveToken_ShouldReturnTrue()
    {
        // Arrange
        var entity = CreateEntity();
        entity.IsActive = true;
        await _tokenWhiteListRepository.AddAsync(entity);
        await _tokenWhiteListRepository.SaveChangesAsync();

        // Act
        var result = await _tokenWhiteListRepository.IsTokenActiveAsync(entity.Jti);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsTokenActiveAsync_InactiveToken_ShouldReturnFalse()
    {
        // Arrange
        var entity = CreateEntity();
        entity.IsActive = false;
        await _tokenWhiteListRepository.AddAsync(entity);
        await _tokenWhiteListRepository.SaveChangesAsync();

        // Act
        var result = await _tokenWhiteListRepository.IsTokenActiveAsync(entity.Jti);

        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task DeactivateTokenAsync_ExistingToken_ShouldDeactivateToken()
    {
        // Arrange
        var entity = CreateEntity();
        entity.IsActive = true;
        await _tokenWhiteListRepository.AddAsync(entity);
        await _tokenWhiteListRepository.SaveChangesAsync();

        // Act
        await _tokenWhiteListRepository.DeactivateTokenAsync(entity.Jti);
        await _tokenWhiteListRepository.SaveChangesAsync();

        // Assert
        var updatedEntity = await _context.TokenWhiteList.FindAsync(entity.Id);
        updatedEntity.Should().NotBeNull();
        updatedEntity.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeactivateTokensByUserIdAsync_UserWithMultipleTokens_ShouldDeactivateAllUserTokens()
    {
        // Arrange
        var user =TestDataFactory.CreateUser(_fixture);
        
        var userToken1 = CreateEntityWithUserId(user.Id);
        userToken1.IsActive = true;
        var userToken2 = CreateEntityWithUserId(user.Id);
        userToken1.IsActive = true;
        
        await _context.TokenWhiteList.AddRangeAsync(userToken1, userToken2);
        await _context.SaveChangesAsync();

        // Act
        await _tokenWhiteListRepository.DeactivateTokensByUserIdAsync(user.Id);
        await _tokenWhiteListRepository.SaveChangesAsync();

        // Assert
        var updatedUserToken1 = await _context.TokenWhiteList.FindAsync(userToken1.Id);
        var updatedUserToken2 = await _context.TokenWhiteList.FindAsync(userToken2.Id);

        updatedUserToken1!.IsActive.Should().BeFalse();
        updatedUserToken2!.IsActive.Should().BeFalse();
    }
    
    [Fact]
    public async Task SaveChangesAsync_WithChanges_ShouldReturnNumberOfChanges()
    {
        // Arrange
        var entity = CreateEntity();
        await _tokenWhiteListRepository.AddAsync(entity);

        // Act
        var result = await _tokenWhiteListRepository.SaveChangesAsync();

        // Assert
        result.Should().Be(1);
    }
}