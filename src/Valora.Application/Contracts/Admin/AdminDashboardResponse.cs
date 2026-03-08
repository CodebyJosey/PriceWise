namespace Valora.Application.Contracts.Admin;

/// <summary>
/// Represents the main admin dashboard response.
/// </summary>
public sealed class AdminDashboardResponse
{
    /// <summary>
    /// Gets or sets the total user count.
    /// </summary>
    public int TotalUsers { get; init; }

    /// <summary>
    /// Gets or sets the total listing count.
    /// </summary>
    public int TotalListings { get; init; }

    /// <summary>
    /// Gets or sets the published listing count.
    /// </summary>
    public int PublishedListings { get; init; }

    /// <summary>
    /// Gets or sets the sold listing count.
    /// </summary>
    public int SoldListings { get; init; }

    /// <summary>
    /// Gets or sets the total prediction count.
    /// </summary>
    public int TotalPredictions { get; init; }

    /// <summary>
    /// Gets or sets the total training run count.
    /// </summary>
    public int TotalTrainingRuns { get; init; }

    /// <summary>
    /// Gets or sets the total audit log count.
    /// </summary>
    public int TotalAuditLogs { get; init; }

    /// <summary>
    /// Gets or sets the latest listing count in the last 24 hours.
    /// </summary>
    public int ListingsLast24Hours { get; init; }

    /// <summary>
    /// Gets or sets the latest prediction count in the last 24 hours.
    /// </summary>
    public int PredictionsLast24Hours { get; init; }

    /// <summary>
    /// Gets or sets the training runs by category.
    /// </summary>
    public required IReadOnlyCollection<AdminMetricItemResponse> TrainingRunsByCategory { get; init; }

    /// <summary>
    /// Gets or sets the predictions by category.
    /// </summary>
    public required IReadOnlyCollection<AdminMetricItemResponse> PredictionsByCategory { get; init; }

    /// <summary>
    /// Gets or sets the listings by category.
    /// </summary>
    public required IReadOnlyCollection<AdminMetricItemResponse> ListingsByCategory { get; init; }

    /// <summary>
    /// Gets or sets the most recent audit logs.
    /// </summary>
    public required IReadOnlyCollection<AdminRecentAuditLogResponse> RecentAuditLogs { get; init; }
}