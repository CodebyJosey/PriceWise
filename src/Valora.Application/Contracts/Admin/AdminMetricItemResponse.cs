namespace Valora.Application.Contracts.Admin;

/// <summary>
/// Represents a named metric item.
/// </summary>
public sealed class AdminMetricItemResponse
{
    /// <summary>
    /// Gets or sets the metric name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the metric value.
    /// </summary>
    public int Value { get; set; }
}