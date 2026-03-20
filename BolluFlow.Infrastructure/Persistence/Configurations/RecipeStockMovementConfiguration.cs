using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class RecipeStockMovementConfiguration : IEntityTypeConfiguration<RecipeStockMovement>
{
    public void Configure(EntityTypeBuilder<RecipeStockMovement> builder)
    {
        builder.ToTable("tb_recipe_stock_movement");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantity).HasColumnType("numeric(18,2)");
        builder.Property(x => x.QuantityInBaseUnit).HasColumnType("numeric(18,2)");
        builder.HasOne(x => x.Recipe).WithMany(x => x.RecipeStockMovements).HasForeignKey(x => x.RecipeId);
    }
}
