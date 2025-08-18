using UrlAlias.Backend.Models;
using Microsoft.EntityFrameworkCore;
using UrlAlias.Backend.Data;

namespace UrlAlias.Backend.Services;

public class AliasService : IAliasService
{
    private readonly AliasDbContext _dbContext;

    public AliasService(AliasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AddResult> AddAsync(AliasEntry entry, CancellationToken cancellationToken = default)
    {
        if (await _dbContext.AliasEntries.AnyAsync(e => e.Alias == entry.Alias, cancellationToken))
            return AddResult.Exists;

        await _dbContext.AliasEntries.AddAsync(entry, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return AddResult.Added;
    }

    public async Task<AliasEntry?> TryGetAsync(string alias, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AliasEntries.FirstOrDefaultAsync(e => e.Alias == alias, cancellationToken);
    }

    public async Task<IEnumerable<AliasEntry>> FindAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AliasEntries
            .AsNoTracking()
            .OrderByDescending(e => e.Alias)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ContinueWith(task => task.Result.AsEnumerable(), cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.AliasEntries
            .AsNoTracking()
            .CountAsync(cancellationToken);
    }
}
