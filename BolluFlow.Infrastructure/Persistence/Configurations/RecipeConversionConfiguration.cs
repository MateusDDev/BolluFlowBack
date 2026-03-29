using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class RecipeConversionConfiguration : IEntityTypeConfiguration<RecipeConversion>
{
    public void Configure(EntityTypeBuilder<RecipeConversion> builder)
    {
        builder.ToTable("tb_recipe_conversion");

        builder.HasKey(x => x.Id);

        builder
            .HasOne(x => x.Recipe)
            .WithMany(x => x.RecipeConversions)
            .HasForeignKey(x => x.RecipeId);
    }
}
