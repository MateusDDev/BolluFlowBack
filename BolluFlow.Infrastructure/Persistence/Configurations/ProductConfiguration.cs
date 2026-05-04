using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("tb_product");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Category)
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(x => x.Description)
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(x => x.ImageUrl)
            .HasColumnType("text")
            .IsRequired(false);

        builder.HasIndex(x => new { x.UserId, x.MinimumStockQuantity, x.StockQuantity });
    }
}
