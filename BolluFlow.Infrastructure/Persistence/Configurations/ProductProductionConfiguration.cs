using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class ProductProductionConfiguration : IEntityTypeConfiguration<ProductProduction>
{
    public void Configure(EntityTypeBuilder<ProductProduction> builder)
    {
        builder.ToTable("product_productions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .HasPrecision(14, 4);

        builder.Property(x => x.Notes)
            .HasMaxLength(255);

        builder
            .HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId);
    }
}
