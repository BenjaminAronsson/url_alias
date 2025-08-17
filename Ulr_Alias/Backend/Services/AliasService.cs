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

    public string? TryGet(string alias)
    {
        
        return _cache.TryGetValue<string>(alias, out var url) ? url : null;
    }

    public AddResult Add(AliasEntry entry)
    {
        if (_cache.TryGetValue<string>(entry.Alias, out _))
            return AddResult.Exists;

        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = entry.ExpiresAt ?? DateTimeOffset.UtcNow.AddHours(12)
        };

        _cache.Set(entry.Alias, entry.Url, options);
        return AddResult.Added;
    }
}
