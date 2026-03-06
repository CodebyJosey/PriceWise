using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valora.Domain.Entities;

namespace Valora.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for <see cref="TrainingRun"/>.
/// </summary>
public sealed class TrainingRunConfiguration : IEntityTypeConfiguration<TrainingRun>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TrainingRun> builder)
    {
        builder.ToTable("TrainingRuns");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CategoryKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DatasetPath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.ModelPath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasIndex(x => x.CategoryKey);
        builder.HasIndex(x => new { x.CategoryKey, x.Version }).IsUnique();
        builder.HasIndex(x => x.CreatedAtUtc);
    }
}