using BolluFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BolluFlow.Infrastructure.Persistence.Configurations;

public class FixedAccountConfiguration : IEntityTypeConfiguration<FixedAccount>
{
    public void Configure(EntityTypeBuilder<FixedAccount> builder)
    {
        builder.ToTable("tb_fixed_account");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DefaultAmount).HasColumnType("numeric(18,2)");

        builder.HasIndex(x => new { x.UserId, x.IsActive });
    }
}
