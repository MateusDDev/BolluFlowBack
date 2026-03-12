using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class SupplyConfiguration : IEntityTypeConfiguration<Supply>
{
    public void Configure(EntityTypeBuilder<Supply> builder)
    {
        builder.ToTable("tb_supply");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.UserId, x.MinimumStockQuantity, x.StockQuantity });
        builder.Property(x => x.BaseUnit).HasConversion<string>();
        builder.HasIndex(x => x.Name);
    }
}
