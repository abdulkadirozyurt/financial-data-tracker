using System;
using System.Collections.Generic;
using System.Text;
using FinancialDataTracker.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace FinancialDataTracker.DataAccess.Concrete.Context;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.OwnsOne(s => s.StockSymbol, sb =>
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
        });

        modelBuilder.Entity<Watchlist>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasColumnName("Name").IsRequired(true);
        });

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Watchlist> Watchlists { get; set; }
}
