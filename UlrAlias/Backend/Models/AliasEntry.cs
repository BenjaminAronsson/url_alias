namespace UlrAlias.Backend.Models;

public class AliasEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Alias { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public int UsageCount { get; set; }
}
