namespace Valora.Application.Contracts.Admin;

/// <summary>
/// Represents a recent audit log entry for the admin dashboard.
/// </summary>
public sealed class AdminRecentAuditLogResponse
{
    /// <summary>
    /// Gets or sets the audit log id.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets or sets the optional user id.
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Gets or sets the action.
    /// </summary>
    public required string Action { get; init; }

    /// <summary>
    /// Gets or sets the entity type.
    /// </summary>
    public required string EntityType { get; init; }

    /// <summary>
    /// Gets or sets the entity id.
    /// </summary>
    public required string EntityId { get; init; }

    /// <summary>
    /// Gets or sets the details.
    /// </summary>
    public required string Details { get; init; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTime CreatedAtUtc { get; init; }
}