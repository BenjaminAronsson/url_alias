namespace UlrAlias.Backend.Extensions;

public static class UrlStringExtension {
    public static string EnsureLeadingSlash(this string? value)
    {
        return "/" + (value ?? string.Empty).TrimStart('/');
    }
}