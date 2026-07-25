using EvTap.Modules.Listings.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvTap.Modules.Listings.Persistence.Configurations;

internal sealed class ListingCommentConfiguration : IEntityTypeConfiguration<ListingComment>
{
    public void Configure(EntityTypeBuilder<ListingComment> builder)
    {
        builder.ToTable("ListingComments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.AuthorName).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Text).HasMaxLength(1000).IsRequired();

        builder.HasIndex(c => c.ListingId);

        builder.HasOne<Listing>()
            .WithMany()
            .HasForeignKey(c => c.ListingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
