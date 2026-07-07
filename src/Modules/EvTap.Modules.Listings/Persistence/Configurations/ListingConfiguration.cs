using EvTap.Modules.Listings.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvTap.Modules.Listings.Persistence.Configurations;

internal sealed class ListingConfiguration : IEntityTypeConfiguration<Listing>
{
    public void Configure(EntityTypeBuilder<Listing> builder)
    {
        builder.ToTable("Listings");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Title).HasMaxLength(200).IsRequired();
        builder.Property(l => l.Description).IsRequired();
        builder.Property(l => l.Price).HasColumnType("numeric(12,2)");
        builder.Property(l => l.District).HasMaxLength(100).IsRequired();
        builder.Property(l => l.Address).HasMaxLength(300).IsRequired();

        builder.Property(l => l.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(l => l.Status);
        builder.HasIndex(l => l.District);
        builder.HasIndex(l => l.OwnerId);

        builder.HasMany(l => l.Images)
            .WithOne()
            .HasForeignKey(i => i.ListingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
