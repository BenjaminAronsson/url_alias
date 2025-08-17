namespace UrlAlias.Models;

public record AliasEntry(string Alias, string Url, DateTimeOffset? ExpiresAt);
