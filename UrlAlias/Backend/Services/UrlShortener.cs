namespace UrlAlias.Backend.Services;

public interface IUrlShortener
{
    string GenerateAlias(string url);
}

public class UrlShortener : IUrlShortener
{
    private static ulong _counter;
    private static readonly object _lock = new();

    public string GenerateAlias(string url)
    {
        ulong id;
        lock (_lock)
        {
            id = ++_counter;
        }

        return Base62Encode(id);
    }

    private static string Base62Encode(ulong value)
    {
        const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        Span<char> buffer = stackalloc char[11]; // Enough for base62 of ulong
        var i = buffer.Length;
        do
        {
            buffer[--i] = chars[(int)(value % 62)];
            value /= 62;
        } while (value > 0);

        return new string(buffer[i..]);
    }
}