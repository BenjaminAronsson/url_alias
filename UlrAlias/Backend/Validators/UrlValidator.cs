namespace UlrAlias.Backend.Validators;

public static class UrlValidator
{
    public static bool IsValid(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult)) return false;
        return uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
    }
}