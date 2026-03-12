using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class SupplyConversionConfiguration : IEntityTypeConfiguration<SupplyConversion>
{
    public void Configure(EntityTypeBuilder<SupplyConversion> builder)
    {
        builder.ToTable("tb_supply_conversion");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Supply)
            .WithMany(x => x.SupplyConversions)
            .HasForeignKey(x => x.SupplyId);
    }
}
