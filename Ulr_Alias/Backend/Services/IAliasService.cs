using UrlAlias.Models;

namespace Ulr_Alias.Backend.Services;

public interface IAliasService
{
    AddResult Add(AliasEntry entry);
    string? TryGet(string alias);
}
