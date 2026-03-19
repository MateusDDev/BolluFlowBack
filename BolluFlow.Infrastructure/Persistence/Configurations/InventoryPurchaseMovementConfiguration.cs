using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class InventoryPurchaseMovementConfiguration : IEntityTypeConfiguration<InventoryPurchaseMovement>
{
    public void Configure(EntityTypeBuilder<InventoryPurchaseMovement> builder)
    {
        builder.ToTable("tb_inventory_purchase_movement");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ItemType).HasConversion<string>();
        builder.Property(x => x.QuantityAdded).HasColumnType("numeric(18,2)");
        builder.Property(x => x.UnitCost).HasColumnType("numeric(18,2)");
        builder.Property(x => x.TotalAmount).HasColumnType("numeric(18,2)");

        builder.HasIndex(x => new { x.UserId, x.Date });
        builder.HasIndex(x => new { x.UserId, x.ItemType, x.ItemId });
    }
}
