using System.ComponentModel;
using UrlAlias.Backend.Models;

namespace UrlAlias.Backend.DTos;

public class AliasEntryDto
{
    [DefaultValue("demo")] 
    public string Alias { get; set; } = string.Empty;

    [DefaultValue("https://www.google.com")]
    public required string Url { get; init; }

    [DefaultValue("2025-12-31T23:59:59Z")] // Fixed future date for Swagger default
    public DateTimeOffset? ExpiresAt { get; init; }

    public AliasEntry ToDomain()
    {
        return new AliasEntry(Alias, Url, ExpiresAt);
    }
}