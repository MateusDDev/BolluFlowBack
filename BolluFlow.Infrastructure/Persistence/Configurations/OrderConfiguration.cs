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

        builder.HasIndex(x => new { x.CustomerId, x.Status, x.CreatedAt });
        builder.HasIndex(x => new { x.UserId, x.Status, x.CreatedAt });
        builder.HasIndex(x => new { x.UserId, x.DeliveredAt });
        
        builder.Property(x => x.Status).HasConversion<string>();
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.PaymentMethod).HasConversion<string>();
        builder.Property(x => x.DiscountType).HasConversion<string>();
        builder.Property(x => x.ShippingFee).HasDefaultValue(0);

        builder.Property(x => x.ChangeFor).HasColumnType("numeric(10,2)");
        
        builder
            .HasOne<Customer>(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne<User>(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
    }
}
