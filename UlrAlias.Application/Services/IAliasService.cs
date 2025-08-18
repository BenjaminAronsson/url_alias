using UlrAlias.Domain.Models;

namespace UlrAlias.Application.Services;

public interface IAliasService
{
    Task<AddResult> AddAsync(AliasEntry entry, CancellationToken cancellationToken = default);
    Task<AliasEntry?> TryGetAsync(string alias, CancellationToken cancellationToken = default);
    Task<IEnumerable<AliasEntry>> FindAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}