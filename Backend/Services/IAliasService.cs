using UrlAlias.Models;

namespace UrlAlias.Services;

public interface IAliasService
{
    AddResult Add(AliasEntry entry);
    string TryGet(string alias);
}
