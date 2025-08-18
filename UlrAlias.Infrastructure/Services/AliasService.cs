using Microsoft.EntityFrameworkCore;
using System.Linq;
using UlrAlias.Application.Services;
using UlrAlias.Domain.Models;
using UlrAlias.Infrastructure.Data;

namespace UlrAlias.Infrastructure.Services;

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
        var duplicates = await _dbContext.AliasEntries
            .Where(e => e.Alias == entry.Alias)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        if (duplicates.Any(e => e.ExpiresAt == null || e.ExpiresAt > now))
            return AddResult.Exists;

        await _dbContext.AliasEntries.AddAsync(entry, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return AddResult.Added;
    }

    public async Task<AliasEntry?> TryGetAsync(string alias, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _dbContext.AliasEntries
            .FirstOrDefaultAsync(e => e.Alias == alias && (e.ExpiresAt == null || e.ExpiresAt > now), cancellationToken);
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
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _dbContext.AliasEntries
            .AsNoTracking()
            .CountAsync(e => e.ExpiresAt == null || e.ExpiresAt > now, cancellationToken);
    }
}
