using EvTap.Modules.Listings.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvTap.Modules.Listings.Persistence.Configurations;

internal sealed class ListingImageConfiguration : IEntityTypeConfiguration<ListingImage>
{
    public void Configure(EntityTypeBuilder<ListingImage> builder)
    {
        builder.ToTable("ListingImages");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Url).HasMaxLength(500).IsRequired();

        builder.HasIndex(i => i.ListingId);
    }
}
