using System;

namespace Ulr_Alias.Models;

public record AliasEntry(string? Alias, string Url, DateTimeOffset? ExpiresAt = null);
