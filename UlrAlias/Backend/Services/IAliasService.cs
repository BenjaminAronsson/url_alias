using UrlAlias.Models;

namespace Ulr_Alias.Backend.Services;

public interface IAliasService
{
    Task<AddResult> AddAsync(AliasEntry entry, CancellationToken cancellationToken = default);
    Task<AliasEntry?> TryGetAsync(string alias, CancellationToken cancellationToken = default);
}
