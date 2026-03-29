using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class FinancialEntryConfiguration : IEntityTypeConfiguration<FinancialEntry>
{
    public void Configure(EntityTypeBuilder<FinancialEntry> builder)
    {
        builder.ToTable("financial_entries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type).HasConversion<string>();
        builder.Property(x => x.Origin).HasConversion<string>();
        builder.Property(x => x.Amount).HasPrecision(14, 2);
        builder.Property(x => x.SourceEvent).HasMaxLength(80);
        builder.Property(x => x.EventKey).HasMaxLength(120);

        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.Type);
        builder.HasIndex(x => new { x.Origin, x.OriginId });
        builder.HasIndex(x => x.EventKey).IsUnique();
    }
}
