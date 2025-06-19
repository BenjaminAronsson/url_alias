using Ulr_Alias.Models;

namespace Ulr_Alias.Services;

public interface IAliasService
{
    AddResult Add(AliasEntry entry);
    string TryGet(string alias);
}
