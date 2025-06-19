using System.Text.Json;

namespace UrlAlias;

public record AliasEntry(string Alias, string Url);
public enum AddResult { Added, Exists }

public class AliasService
{
    private readonly string _dataFile;
    private readonly object _lock = new();
    public AliasService(string dataFile)
    {
        _dataFile = dataFile;
    }

    public IDictionary<string, string> GetAll()
    {
        lock (_lock)
        {
            return Load();
        }
    }

    public string? TryGet(string alias)
    {
        lock (_lock)
        {
            var aliases = Load();
            return aliases.TryGetValue(alias, out var url) ? url : null;
        }
    }

    public AddResult Add(AliasEntry entry)
    {
        lock (_lock)
        {
            var aliases = Load();
            if (aliases.ContainsKey(entry.Alias))
                return AddResult.Exists;
            aliases[entry.Alias] = entry.Url;
            Save(aliases);
            return AddResult.Added;
        }
    }

    private Dictionary<string, string> Load()
    {
        if (File.Exists(_dataFile))
        {
            try
            {
                var json = File.ReadAllText(_dataFile);
                return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
            }
            catch
            {
                return new Dictionary<string, string>();
            }
        }
        return new Dictionary<string, string>();
    }

    private void Save(Dictionary<string, string> aliases)
    {
        var json = JsonSerializer.Serialize(aliases, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_dataFile, json);
    }
}
