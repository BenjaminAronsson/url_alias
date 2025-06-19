using System;
using System.ComponentModel;
using Ulr_Alias.Models;

namespace Ulr_Alias.Dtos;

public class AliasEntryDto
{
    [DefaultValue("demo")]
    public string? Alias { get; set; }

    [DefaultValue("https://www.google.com")]
    public required string Url { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }

    public AliasEntry ToDomain() => new AliasEntry(Alias, Url, ExpiresAt);
}
