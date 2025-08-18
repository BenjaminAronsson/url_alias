using UlrAlias.Backend.Models;
using Microsoft.EntityFrameworkCore;
using UlrAlias.Backend.Data;

namespace UlrAlias.Backend.Services;

public class AliasService : IAliasService
{
    private readonly AliasDbContext _dbContext;

    public AliasService(AliasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AddResult> AddAsync(AliasEntry entry, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var exists = await _dbContext.AliasEntries
            .Where(e => e.Alias == entry.Alias)
            .Where(e => e.ExpiresAt == null || e.ExpiresAt > now)
            .AnyAsync(cancellationToken);
        if (exists)
            return AddResult.Exists;

        await _dbContext.AliasEntries.AddAsync(entry, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return AddResult.Added;
    }

    public async Task<AliasEntry?> TryGetAsync(string alias, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var entry = await _dbContext.AliasEntries
            .Where(e => e.Alias == alias)
            .Where(e => e.ExpiresAt == null || e.ExpiresAt > now)
            .FirstOrDefaultAsync(cancellationToken);
        if (entry is not null)
        {
            entry.UsageCount++;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        return entry;
    }

    public async Task<IEnumerable<AliasEntry>> FindAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _dbContext.AliasEntries
            .AsNoTracking()
            .Where(e => e.ExpiresAt == null || e.ExpiresAt > now)
            .OrderByDescending(e => e.Alias)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ContinueWith(task => task.Result.AsEnumerable(), cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _dbContext.AliasEntries
            .AsNoTracking()
            .Where(e => e.ExpiresAt == null || e.ExpiresAt > now)
            .CountAsync(cancellationToken);
    }
}
