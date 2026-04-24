using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class UserStockUnitPreferenceConfiguration : IEntityTypeConfiguration<UserStockUnitPreference>
{
    public void Configure(EntityTypeBuilder<UserStockUnitPreference> builder)
    {
        builder.ToTable("user_stock_unit_preferences");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ItemType)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(x => new { x.UserId, x.ItemType, x.ItemId })
            .IsUnique();
    }
}
