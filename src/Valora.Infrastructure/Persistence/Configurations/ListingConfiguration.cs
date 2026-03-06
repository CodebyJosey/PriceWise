using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valora.Domain.Entities;

namespace Valora.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for <see cref="Listing"/>.
/// </summary>
public sealed class ListingConfiguration : IEntityTypeConfiguration<Listing>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Listing> builder)
    {
        builder.ToTable("Listings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.CategoryKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.AskingPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.PredictedPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.FeaturesJson)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.OwnerUserId);
        builder.HasIndex(x => x.CategoryKey);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAtUtc);
    }
}