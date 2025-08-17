using System.ComponentModel;
using UrlAlias.Models;

namespace UrlAlias.Dtos;

public class AliasEntryRequest
{
    [DefaultValue("demo")]
    public string? Alias { get; set; }

    [DefaultValue("https://www.google.com")]
    public required string Url { get; set; }
    
    [DefaultValue("2025-12-31T23:59:59Z")] // Fixed future date for Swagger default
    public DateTimeOffset? ExpiresAt { get; set; }

    public AliasEntry ToDomain() => new AliasEntry(Alias, Url, ExpiresAt);
}

