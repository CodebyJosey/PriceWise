namespace Valora.Application.Abstractions.Logging;

/// <summary>
/// Service for writing audit logs.
/// </summary>
public interface IAuditLogService
{
    /// <summary>
    /// Writes an audit log entry.
    /// </summary>
    /// <param name="userId">The optional user id.</param>
    /// <param name="action">The action name.</param>
    /// <param name="entityType">The entity type.</param>
    /// <param name="entityId">The optional entity id.</param>
    /// <param name="details">Additional details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task WriteAsync(
        Guid? userId,
        string action,
        string entityType,
        string? entityId,
        string? details,
        CancellationToken cancellationToken = default);
}