using EvTap.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvTap.Modules.Users.Persistence.Configurations;

internal sealed class EmailOtpConfiguration : IEntityTypeConfiguration<EmailOtp>
{
    public void Configure(EntityTypeBuilder<EmailOtp> builder)
    {
        builder.ToTable("EmailOtps");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Email).HasMaxLength(320).IsRequired();
        builder.Property(o => o.Code).HasMaxLength(6).IsRequired();

        builder.HasIndex(o => o.Email);
    }
}
