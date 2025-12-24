using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("tb_order");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Status).HasConversion<string>();
        
        builder
            .HasOne<Customer>(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId);
    }
}