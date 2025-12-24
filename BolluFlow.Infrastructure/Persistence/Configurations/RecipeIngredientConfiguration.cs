using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("tb_recipe_ingredient");

        builder.HasKey(x => new { x.RecipeId, x.IngredientId });
        
        builder
            .HasOne<Recipe>(x => x.Recipe)
            .WithMany(x => x.RecipeIngredients)
            .HasForeignKey(x => x.RecipeId);
        
        builder
            .HasOne<Ingredient>(x => x.Ingredient)
            .WithMany(x => x.RecipeIngredients)
            .HasForeignKey(x => x.IngredientId);
    }
}