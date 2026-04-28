using System;
using System.Collections.Generic;
using System.Text;
using FinancialDataTracker.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace FinancialDataTracker.DataAccess.Concrete.Context;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.OwnsOne(s => s.StockSymbol, sb =>
            {
                sb.Property(s => s.Symbol).HasColumnName("Symbol").IsRequired(false);
                sb.HasIndex(s => s.Symbol).IsUnique();

                sb.Property(s => s.DisplaySymbol).HasColumnName("DisplaySymbol").IsRequired(false);
                sb.Property(s => s.Description).HasColumnName("Description").IsRequired(false);
                sb.Property(s => s.Currency).HasColumnName("Currency").IsRequired(false);
            });
        });

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Stock> Stocks { get; set; }
}
