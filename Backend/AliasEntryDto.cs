using System;

namespace UrlAlias;

public class AliasEntryDto
{
    public string Alias { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTimeOffset? ExpiresAt { get; set; }

    public AliasEntry ToDomain() => new AliasEntry(Alias, Url, ExpiresAt);
}
