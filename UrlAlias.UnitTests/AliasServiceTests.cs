using Microsoft.EntityFrameworkCore;
using UrlAlias.Backend.Models;
using UrlAlias.Backend.Services;
using UrlAlias.Backend.Data;

namespace UrlAlias.UnitTests;

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
        var entry = new AliasEntry("test", "https://example.com", null);

        var result = await service.AddAsync(entry);

        Assert.Equal(AddResult.Added, result);
    }

    [Fact]
    public async Task Add_ReturnsExists_WhenAliasAlreadyExists()
    {
        var dbContext = CreateInMemoryDbContext();
        var service = new AliasService(dbContext);
        var entry = new AliasEntry("test", "https://example.com", null);
        await service.AddAsync(entry);

        var result = await service.AddAsync(entry);

        Assert.Equal(AddResult.Exists, result);
    }
}
