using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("tb_user");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.WhatsappMessageTemplate)
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(x => x.IndirectCostRate)
            .HasColumnType("numeric(5,2)")
            .IsRequired(false);

        builder.Property(x => x.SkipStockCheck)
            .HasDefaultValue(false)
            .IsRequired();
    }
}
