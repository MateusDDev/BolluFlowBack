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

        builder.Property(x => x.Slug)
            .HasColumnType("varchar(255)")
            .IsRequired(false);

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.Property(x => x.Whatsapp)
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(x => x.WhatsappMessageTemplate)
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(x => x.PixKey)
            .HasColumnType("varchar(140)")
            .IsRequired(false);

        builder.OwnsOne(x => x.StoreAddress, address =>
        {
            address.Property(x => x.Street).HasColumnName("store_street").HasColumnType("varchar(200)");
            address.Property(x => x.Number).HasColumnName("store_number");
            address.Property(x => x.Neighborhood).HasColumnName("store_neighborhood").HasColumnType("varchar(120)");
            address.Property(x => x.Complement).HasColumnName("store_complement").HasColumnType("varchar(200)");
        });

        builder.Property(x => x.IndirectCostRate)
            .HasColumnType("numeric(5,2)")
            .IsRequired(false);
    }
}
