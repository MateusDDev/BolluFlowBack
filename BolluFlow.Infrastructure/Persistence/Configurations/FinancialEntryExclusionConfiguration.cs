using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class FinancialEntryExclusionConfiguration : IEntityTypeConfiguration<FinancialEntryExclusion>
{
    public void Configure(EntityTypeBuilder<FinancialEntryExclusion> builder)
    {
        builder.ToTable("tb_financial_entry_exclusion");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.FinancialEntryId).IsUnique();

        builder.HasOne(x => x.FinancialEntry)
            .WithMany()
            .HasForeignKey(x => x.FinancialEntryId);
    }
}
