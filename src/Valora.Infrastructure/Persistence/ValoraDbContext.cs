using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Valora.Domain.Entities;
using Valora.Infrastructure.Persistence.Identity;

namespace Valora.Infrastructure.Persistence;

/// <summary>
/// Main EF Core database context for Valora.
/// </summary>
public class ValoraDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValoraDbContext"/> class.
    /// </summary>
    /// <param name="options">The DbContext options.</param>   
    public ValoraDbContext(DbContextOptions<ValoraDbContext> options) : base(options)
    { }

    /// <summary>
    /// Gets the listings table.
    /// </summary>
    public DbSet<Listing> Listings => Set<Listing>();

    /// <summary>
    /// Gets the prediction records table.
    /// </summary>
    public DbSet<PredictionRecord> PredictionRecords => Set<PredictionRecord>();

    /// <summary>
    /// Gets the training runs table.
    /// </summary>
    public DbSet<TrainingRun> TrainingRuns => Set<TrainingRun>();

    /// <summary>
    /// Gets the audit logs table.
    /// </summary>
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ValoraDbContext).Assembly);
    }
}