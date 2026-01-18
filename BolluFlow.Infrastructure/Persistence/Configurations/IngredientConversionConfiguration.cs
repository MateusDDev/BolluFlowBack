using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class IngredientConversionConfiguration : IEntityTypeConfiguration<IngredientConversion>
{
    public void Configure(EntityTypeBuilder<IngredientConversion> builder)
    {
        builder.ToTable("tb_ingredient_conversion");
        
        builder.HasKey(x => x.Id);
        
        builder
            .HasOne(x => x.Ingredient)
            .WithMany()
            .HasForeignKey(x => x.IngredientId);
    }
}