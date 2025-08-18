using Microsoft.EntityFrameworkCore;
using UlrAlias.Backend.Models;
using UlrAlias.Backend.Services;
using UlrAlias.Backend.Data;

namespace UlrAlias.UnitTests;

public class AliasServiceTests
{
    private static AliasDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AliasDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        var dbContext = new AliasDbContext(options);
        dbContext.Database.OpenConnection();
        dbContext.Database.EnsureCreated();
        return dbContext;
    }

    [Fact]
    public async Task TryGet_ReturnsNull_WhenAliasNotFound()
    {
        var dbContext = CreateInMemoryDbContext();
        var service = new AliasService(dbContext);

        var result = await service.TryGetAsync("nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task Add_ReturnsAdded_WhenAliasDoesNotExist()
    {
        var dbContext = CreateInMemoryDbContext();
        var service = new AliasService(dbContext);
        var entry = new AliasEntry { Alias = "test", Url = "https://example.com", UserId = "user1" };

        var result = await service.AddAsync(entry);

        Assert.Equal(AddResult.Added, result);
    }

    [Fact]
    public async Task Add_ReturnsExists_WhenAliasAlreadyExists()
    {
        var dbContext = CreateInMemoryDbContext();
        var service = new AliasService(dbContext);
        var entry = new AliasEntry { Alias = "test", Url = "https://example.com", UserId = "user1" };
        await service.AddAsync(entry);

        var result = await service.AddAsync(entry);

        Assert.Equal(AddResult.Exists, result);
    }

    [Fact]
    public async Task Add_AllowsDuplicate_WhenExistingAliasExpired()
    {
        var dbContext = CreateInMemoryDbContext();
        var service = new AliasService(dbContext);
        var expired = new AliasEntry
        {
            Alias = "test",
            Url = "https://example.com",
            UserId = "user1",
            ExpiresAt = DateTime.UtcNow.AddDays(-1)
        };
        await service.AddAsync(expired);

        var entry = new AliasEntry { Alias = "test", Url = "https://example.com/new", UserId = "user2" };

        var result = await service.AddAsync(entry);

        Assert.Equal(AddResult.Added, result);
    }

    [Fact]
    public async Task TryGet_IncrementsUsageCount()
    {
        var dbContext = CreateInMemoryDbContext();
        var service = new AliasService(dbContext);
        var entry = new AliasEntry { Alias = "test", Url = "https://example.com", UserId = "user1" };
        await service.AddAsync(entry);

        await service.TryGetAsync("test");
        var result = await service.TryGetAsync("test");

        Assert.Equal(2, result?.UsageCount);
    }

    [Fact]
    public async Task TryGet_ReturnsNull_WhenAliasExpired()
    {
        var dbContext = CreateInMemoryDbContext();
        var service = new AliasService(dbContext);
        var expired = new AliasEntry
        {
            Alias = "test",
            Url = "https://example.com",
            UserId = "user1",
            ExpiresAt = DateTime.UtcNow.AddDays(-1)
        };
        await service.AddAsync(expired);

        var result = await service.TryGetAsync("test");

        Assert.Null(result);
    }
}
