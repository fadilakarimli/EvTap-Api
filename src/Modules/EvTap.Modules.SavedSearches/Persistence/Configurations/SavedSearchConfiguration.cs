using EvTap.Modules.SavedSearches.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvTap.Modules.SavedSearches.Persistence.Configurations;

internal sealed class SavedSearchConfiguration : IEntityTypeConfiguration<SavedSearch>
{
    public void Configure(EntityTypeBuilder<SavedSearch> builder)
    {
        builder.ToTable("SavedSearches");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.MinPrice).HasColumnType("numeric(12,2)");
        builder.Property(s => s.MaxPrice).HasColumnType("numeric(12,2)");
        builder.Property(s => s.District).HasMaxLength(100);

        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.IsActive);
    }
}
