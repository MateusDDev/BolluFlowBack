using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class FinancialEntryConfiguration : IEntityTypeConfiguration<FinancialEntry>
{
    public void Configure(EntityTypeBuilder<FinancialEntry> builder)
    {
        builder.ToTable("tb_financial_entries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type).HasConversion<string>();
        builder.Property(x => x.Origin).HasConversion<string>();
        builder.Property(x => x.Amount).HasPrecision(14, 2);
        builder.Property(x => x.Description).HasMaxLength(255);
        builder.Property(x => x.EventKey).HasMaxLength(120);

        builder.HasIndex(x => new { x.UserId, x.Type, x.CreatedAt });
        builder.HasIndex(x => new { x.UserId, x.Origin, x.OriginId });
        builder.HasIndex(x => new { x.UserId, x.EventKey }).IsUnique();
    }
}
