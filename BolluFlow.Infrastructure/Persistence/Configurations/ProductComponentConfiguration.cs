using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class ProductComponentConfiguration : IEntityTypeConfiguration<ProductComponent>
{
    public void Configure(EntityTypeBuilder<ProductComponent> builder)
    {
        builder.ToTable("tb_product_component");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ComponentType).HasConversion<string>();
        builder.HasIndex(x => x.ProductId);

        builder.HasOne(x => x.Product).WithMany(x => x.Components).HasForeignKey(x => x.ProductId);
        builder.HasOne(x => x.Recipe).WithMany().HasForeignKey(x => x.RecipeId);
        builder.HasOne(x => x.Ingredient).WithMany().HasForeignKey(x => x.IngredientId);
        builder.HasOne(x => x.Supply).WithMany().HasForeignKey(x => x.SupplyId);
    }
}
