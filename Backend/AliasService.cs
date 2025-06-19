using Microsoft.Extensions.Caching.Memory;

namespace UrlAlias;

public record AliasEntry(string Alias, string Url, DateTimeOffset? ExpiresAt = null);
public enum AddResult { Added, Exists }

public class AliasService
{
    private readonly IMemoryCache _cache;
    private readonly object _lock = new();
    private readonly HashSet<string> _keys = new();

    public AliasService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public IDictionary<string, string> GetAll()
    {
        lock (_lock)
        {
            var result = new Dictionary<string, string>();
            var remove = new List<string>();
            foreach (var key in _keys)
            {
                if (_cache.TryGetValue<string>(key, out var url))
                    result[key] = url!;
                else
                    remove.Add(key);
            }
            foreach (var r in remove)
                _keys.Remove(r);
            return result;
        }
    }

    public string? TryGet(string alias)
    {
        lock (_lock)
        {
            return _cache.TryGetValue<string>(alias, out var url) ? url : null;
        }
    }

    public AddResult Add(AliasEntry entry)
    {
        lock (_lock)
        {
            if (_cache.TryGetValue<string>(entry.Alias, out _))
                return AddResult.Exists;

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = entry.ExpiresAt ?? DateTimeOffset.UtcNow.AddHours(12)
            };
            options.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                lock (_lock)
                {
                    _keys.Remove((string)key);
                }
            });

            _cache.Set(entry.Alias, entry.Url, options);
            _keys.Add(entry.Alias);
            return AddResult.Added;
        }
    }
}
