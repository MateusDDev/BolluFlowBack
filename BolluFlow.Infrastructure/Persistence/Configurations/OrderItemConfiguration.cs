using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("tb_order_item");

        builder.HasKey(x => x.Id);
        
        builder
            .HasOne(x => x.Order)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.OrderId);
        
        builder
            .HasOne(x => x.Recipe)
            .WithMany()
            .HasForeignKey(x => x.RecipeId);
    }
}