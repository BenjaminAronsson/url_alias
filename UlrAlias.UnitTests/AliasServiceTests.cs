using Microsoft.EntityFrameworkCore;
using System.Linq;
using UlrAlias.Domain.Models;
using UlrAlias.Infrastructure.Services;
using UlrAlias.Infrastructure.Data;

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
        var entry = new AliasEntry { Alias = "test", Url = "https://example.com" };

        var result = await service.AddAsync(entry);

        Assert.Equal(AddResult.Added, result);
    }

    [Fact]
    public async Task Add_ReturnsExists_WhenAliasAlreadyExists()
    {
        var dbContext = CreateInMemoryDbContext();
        var service = new AliasService(dbContext);
        var entry = new AliasEntry { Alias = "test", Url = "https://example.com" };
        await service.AddAsync(entry);

        var result = await service.AddAsync(new AliasEntry { Alias = "test", Url = "https://example.com/other" });

        Assert.Equal(AddResult.Exists, result);
    }

    [Fact]
    public async Task Add_AllowsReuse_WhenExistingAliasExpired()
    {
        var dbContext = CreateInMemoryDbContext();
        var service = new AliasService(dbContext);
        await service.AddAsync(new AliasEntry { Alias = "reuse", Url = "https://example.com", ExpiresAt = DateTime.UtcNow.AddMinutes(-1) });

        var result = await service.AddAsync(new AliasEntry { Alias = "reuse", Url = "https://example.com/new" });

        Assert.Equal(AddResult.Added, result);
    }

    [Fact]
    public async Task TryGet_ReturnsNull_WhenAliasExpired()
    {
        var dbContext = CreateInMemoryDbContext();
        var service = new AliasService(dbContext);
        var expired = new AliasEntry { Alias = "expired", Url = "https://example.com", ExpiresAt = DateTime.UtcNow.AddMinutes(-1) };
        await service.AddAsync(expired);

        var result = await service.TryGetAsync("expired");

        Assert.Null(result);
    }

    [Fact]
    public async Task FindAsync_ExcludesExpiredAliases()
    {
        var dbContext = CreateInMemoryDbContext();
        var service = new AliasService(dbContext);
        await service.AddAsync(new AliasEntry { Alias = "active", Url = "https://example.com" });
        await service.AddAsync(new AliasEntry { Alias = "expired", Url = "https://example.com", ExpiresAt = DateTime.UtcNow.AddMinutes(-1) });

        var results = await service.FindAsync(0, 10);

        var aliasEntries = results.ToList();
        Assert.Single(aliasEntries);
        Assert.Equal("active", aliasEntries[0].Alias);
    }

    [Fact]
    public async Task CountAsync_ExcludesExpiredAliases()
    {
        var dbContext = CreateInMemoryDbContext();
        var service = new AliasService(dbContext);
        await service.AddAsync(new AliasEntry { Alias = "active", Url = "https://example.com" });
        await service.AddAsync(new AliasEntry { Alias = "expired", Url = "https://example.com", ExpiresAt = DateTime.UtcNow.AddMinutes(-1) });

        var count = await service.CountAsync();

        Assert.Equal(1, count);
    }
}
