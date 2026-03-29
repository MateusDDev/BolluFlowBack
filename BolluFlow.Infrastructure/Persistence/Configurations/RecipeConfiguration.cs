using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.ToTable("tb_recipe");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.BaseUnit).HasConversion<string>();

        builder.Property(x => x.StockQuantity)
            .HasPrecision(14, 4)
            .HasDefaultValue(0);

        builder.Property(x => x.MinimumStockQuantity)
            .HasPrecision(14, 4);
    }
}
