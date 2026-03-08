using Valora.Application.Abstractions.Logging;
using Valora.Domain.Entities;
using Valora.Infrastructure.Persistence;

namespace Valora.Infrastructure.Services.Logging;

/// <summary>
/// Default database-backed audit log service.
/// </summary>
public sealed class AuditLogService : IAuditLogService
{
    private readonly ValoraDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditLogService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public AuditLogService(ValoraDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task WriteAsync(
        Guid? userId,
        string action,
        string entityType,
        string? entityId,
        string? details,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            throw new ArgumentException("Action is required.", nameof(action));
        }

        if (string.IsNullOrWhiteSpace(entityType))
        {
            throw new ArgumentException("EntityType is required.", nameof(entityType));
        }

        AuditLog log = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = Trim(action, 200),
            EntityType = Trim(entityType, 200),
            EntityId = Trim(entityId ?? string.Empty, 200),
            Details = Trim(details ?? string.Empty, 8000),
            CreatedAtUtc = DateTime.UtcNow
        };

        _dbContext.AuditLogs.Add(log);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string Trim(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value.Length <= maxLength
            ? value
            : value[..maxLength];
    }
}