using EvTap.Modules.Notifications.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvTap.Modules.Notifications.Persistence.Configurations;

internal sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Channel)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(n => n.UserId);

        // Idempotency guard: one notification per user+listing+channel, enforced at the
        // database level so concurrent consumers cannot double-notify.
        builder.HasIndex(n => new { n.UserId, n.ListingId, n.Channel }).IsUnique();
    }
}
