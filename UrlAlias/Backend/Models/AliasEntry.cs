namespace UrlAlias.Backend.Models;

public record AliasEntry(string Alias, string Url, DateTimeOffset? ExpiresAt);