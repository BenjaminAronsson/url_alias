using Microsoft.Extensions.Caching.Memory;
using UlrAlias.Backend.Models;
using UlrAlias.Backend.Services;

namespace UlrAlias.UnitTests;

public class AliasServiceTests
{
    [Fact]
    public async Task TryGet_ReturnsNull_WhenAliasNotFound()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new AliasService(cache);

        var result = await service.TryGetAsync("nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task Add_ReturnsAdded_WhenAliasDoesNotExist()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new AliasService(cache);
        var entry = new AliasEntry("test", "https://example.com", null);

        var result = await service.AddAsync(entry);

        Assert.Equal(AddResult.Added, result);
    }

    [Fact]
    public async Task Add_ReturnsExists_WhenAliasAlreadyExists()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new AliasService(cache);
        var entry = new AliasEntry("test", "https://example.com", null);
        await service.AddAsync(entry);

        var result = await service.AddAsync(entry);

        Assert.Equal(AddResult.Exists, result);
    }
}