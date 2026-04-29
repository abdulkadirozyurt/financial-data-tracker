using FinancialDataTracker.Entities.Abstract;
using FinancialDataTracker.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FinancialDataTracker.DataAccess.Concrete.Context;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.OwnsOne(s => s.StockDetails, sb =>
            {
                sb.Property(s => s.Symbol).HasColumnName("Symbol").IsRequired(true);
                sb.HasIndex(s => s.Symbol).IsUnique();

                sb.Property(s => s.DisplaySymbol).HasColumnName("DisplaySymbol").IsRequired(true);
                sb.Property(s => s.Description).HasColumnName("Description").IsRequired(false);
                sb.Property(s => s.Currency).HasColumnName("Currency").IsRequired(false);
                sb.Property(s => s.Type).HasColumnName("Type").IsRequired(false);
            });

            entity
                .HasMany(s => s.Watchlists)
                .WithMany(w => w.Stocks)
                .UsingEntity("WatchlistStock");

            entity.
                HasMany(s => s.QuoteSnapshots)
                .WithOne(q => q.Stock)
                .HasForeignKey(q => q.StockId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Watchlist>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasColumnName("Name").IsRequired(true);
        });

        modelBuilder.Entity<StockQuoteSnapshot>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.StockId, e.FetchedAtUtc });
            entity.HasIndex(e => new { e.Symbol, e.FetchedAtUtc });

            entity.OwnsOne(e => e.Quote, eq =>
            {
                eq.Property(e => e.CurrentPrice).HasPrecision(18, 4);
                eq.Property(e => e.OpenPrice).HasPrecision(18, 4);
                eq.Property(e => e.HighPrice).HasPrecision(18, 4);
                eq.Property(e => e.LowPrice).HasPrecision(18, 4);
                eq.Property(e => e.PreviousClosePrice).HasPrecision(18, 4);
                eq.Property(e => e.Change).HasPrecision(18, 4);
                eq.Property(e => e.PercentChange).HasPrecision(18, 4);
                eq.Property(e => e.FinnhubTimestampUtc).IsRequired(false);
            });
        });


        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<Entity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(p => p.CreatedAt).CurrentValue = DateTimeOffset.UtcNow;
            }
            if (entry.State == EntityState.Modified)
            {
                if (entry.Property(p => p.IsDeleted).CurrentValue == true)
                {
                    entry.Property(p => p.DeletedAt).CurrentValue = DateTimeOffset.UtcNow;

                }
                else
                {
                    entry.Property(p => p.UpdatedAt).CurrentValue = DateTimeOffset.UtcNow;
                }
            }

            if (entry.State == EntityState.Deleted)
            {
                throw new ArgumentException("You cannot delete an entity directly, use soft delete instead.");
            }
        }


        return base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Watchlist> Watchlists { get; set; }
    public DbSet<StockQuoteSnapshot> StockQuoteSnapshots { get; set; }
}
