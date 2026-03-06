using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Valora.Infrastructure.Persistence.DesignTime;

/// <summary>
/// Design-time factory for creating <see cref="ValoraDbContext"/> during EF Core tooling operations.
/// </summary>
public sealed class ValoraDbContextFactory : IDesignTimeDbContextFactory<ValoraDbContext>
{
    public ValoraDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<ValoraDbContext> builder = new DbContextOptionsBuilder<ValoraDbContext>();

        string connectionString =
            "Host=localhost;Port=5432;Database=valora_db;Username=postgres;Password=1234";

        builder.UseNpgsql(connectionString);

        return new ValoraDbContext(builder.Options);
    }
}