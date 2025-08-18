using Microsoft.EntityFrameworkCore;
using UlrAlias.Backend.Models;

namespace UlrAlias.Backend.Data;

public class AliasDbContext : DbContext
{
    public AliasDbContext(DbContextOptions<AliasDbContext> options) : base(options) { }

    public DbSet<AliasEntry> AliasEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AliasEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Alias);
            entity.Property(e => e.Alias).IsRequired();
            entity.Property(e => e.Url).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.ExpiresAt);
            entity.Property(e => e.UsageCount);
        });
    }
}
