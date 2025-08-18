namespace UlrAlias.Domain.Models;

public class AliasEntry
{
    public int Id { get; set; }
    public string Alias { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
}
