using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class RecipeProductionConfiguration : IEntityTypeConfiguration<RecipeProduction>
{
    public void Configure(EntityTypeBuilder<RecipeProduction> builder)
    {
        builder.ToTable("recipe_productions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .HasPrecision(14, 4);

        builder.Property(x => x.Notes)
            .HasMaxLength(255);

        builder
            .HasOne(x => x.Recipe)
            .WithMany()
            .HasForeignKey(x => x.RecipeId);
    }
}
