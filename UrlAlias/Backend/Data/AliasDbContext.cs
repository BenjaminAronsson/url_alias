using Microsoft.EntityFrameworkCore;
using UrlAlias.Backend.Models;

namespace UrlAlias.Backend.Data;

public class AliasDbContext : DbContext
{
    public AliasDbContext(DbContextOptions<AliasDbContext> options) : base(options) { }

    public DbSet<AliasEntry> AliasEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AliasEntry>(entity =>
        {
            entity.HasKey(e => e.Alias);
            entity.Property(e => e.Url).IsRequired();
            entity.Property(e => e.ExpiresAt);
        });
    }
}
