using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Valora.Infrastructure.Persistence.Identity;

namespace Valora.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for <see cref="ApplicationUser"/>.
/// </summary>
public sealed class AppUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users");

        builder.Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}