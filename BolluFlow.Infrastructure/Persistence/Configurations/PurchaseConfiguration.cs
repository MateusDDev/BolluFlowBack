using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable("tb_purchase");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.ItemType).HasConversion<string>();
        builder.Property(x => x.ItemNameSnapshot).HasMaxLength(200);
        builder.Property(x => x.ConversionNameSnapshot).HasMaxLength(200);
        builder.Property(x => x.Quantity).HasPrecision(12, 3);
        builder.Property(x => x.QuantityInBaseUnit).HasPrecision(12, 3);
        builder.Property(x => x.TotalCost).HasPrecision(12, 2);
        builder.Property(x => x.PurchasedAt).HasColumnType("date");
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.ItemType, x.ItemId, x.DeletedAt });
    }
}
