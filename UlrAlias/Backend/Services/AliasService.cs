using Microsoft.Extensions.Caching.Memory;
using Ulr_Alias.Backend.Services;
using UrlAlias.Models;

namespace UrlAlias.Services;

public class AliasService : IAliasService
{
    private readonly IMemoryCache _cache; //TODO: change to distributed cache or db

    public AliasService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<AddResult> AddAsync(AliasEntry entry, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue<AliasEntry>(entry.Alias, out _))
            return Task.FromResult(AddResult.Exists);

        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = entry.ExpiresAt ?? DateTimeOffset.UtcNow.AddHours(12)
        };

        _cache.Set(entry.Alias, entry, options);
        return Task.FromResult(AddResult.Added);
    }

    public Task<AliasEntry?> TryGetAsync(string alias, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_cache.TryGetValue<AliasEntry>(alias, out var entry) ? entry : null);
    }
}