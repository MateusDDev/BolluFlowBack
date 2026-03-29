using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("stock_movements");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ItemType).HasConversion<string>();
        builder.Property(x => x.MovementType).HasConversion<string>();

        builder.Property(x => x.Quantity).HasPrecision(14, 4);
        builder.Property(x => x.UnitCost).HasPrecision(14, 4);
        builder.Property(x => x.TotalValue).HasPrecision(14, 2);

        builder.Property(x => x.EventKey).HasMaxLength(120);
        builder.Property(x => x.ReferenceType).HasMaxLength(40);

        builder.HasIndex(x => new { x.ItemType, x.ItemId, x.CreatedAt });
        builder.HasIndex(x => new { x.MovementType, x.CreatedAt });
        builder.HasIndex(x => x.EventKey).IsUnique();
    }
}
