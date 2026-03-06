using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valora.Domain.Entities;

namespace Valora.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for <see cref="PredictionRecord"/>.
/// </summary>
public sealed class PredictionRecordConfiguration : IEntityTypeConfiguration<PredictionRecord>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PredictionRecord> builder)
    {
        builder.ToTable("PredictionRecords");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CategoryKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.FeaturesJson)
            .IsRequired();

        builder.Property(x => x.PredictedPrice)
            .HasPrecision(18, 2);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.CategoryKey);
        builder.HasIndex(x => x.CreatedAtUtc);
    }
}