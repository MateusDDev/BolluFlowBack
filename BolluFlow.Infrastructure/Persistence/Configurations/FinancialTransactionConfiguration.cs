using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class FinancialTransactionConfiguration : IEntityTypeConfiguration<FinancialTransaction>
{
    public void Configure(EntityTypeBuilder<FinancialTransaction> builder)
    {
        builder.ToTable("tb_financial_transaction");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Type).HasConversion<string>();
        builder.Property(x => x.SourceType).HasConversion<string>();
        builder.Property(x => x.SourceEntityType).HasConversion<string>();
        builder.Property(x => x.Amount).HasPrecision(12, 2);
        builder.Property(x => x.EventDate).HasColumnType("date");
        builder.Property(x => x.Description).HasMaxLength(250);
        builder.Property(x => x.Month).HasMaxLength(7);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.Month, x.Type, x.DeletedAt });
        builder.HasIndex(x => new { x.SourceEntityType, x.SourceEntityId });
    }
}
