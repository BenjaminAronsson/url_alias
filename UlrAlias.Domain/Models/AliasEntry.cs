namespace UlrAlias.Domain.Models;

public record AliasEntry(string Alias, string Url, DateTimeOffset? ExpiresAt);