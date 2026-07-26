using EvTap.Modules.Bookings.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvTap.Modules.Bookings.Persistence.Configurations;

internal sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.ListingTitle).HasMaxLength(200).IsRequired();
        builder.Property(b => b.RenterEmail).HasMaxLength(320).IsRequired();
        builder.Property(b => b.PaymentReference).HasMaxLength(64);
        builder.Property(b => b.CardLast4).HasMaxLength(4);

        builder.Property(b => b.UnitPriceAzn).HasPrecision(12, 2);
        builder.Property(b => b.TotalPriceAzn).HasPrecision(12, 2);

        builder.Property(b => b.RentalType).HasConversion<string>().HasMaxLength(20);
        builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(b => b.RenterId);
    }
}
