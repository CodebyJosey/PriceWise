namespace PriceWise.DatasetGenerator.Models;

/// <summary>
/// Result of a dataset generation run.
/// </summary>
public sealed class DatasetGenerationResult
{
    /// <summary>
    /// Gets or sets the category key.
    /// </summary>
    public required string CategoryKey { get; init; }

    /// <summary>
    /// Gets or sets the output file path.
    /// </summary>
    public required string OutputPath { get; init; }

    /// <summary>
    /// Gets or sets the number of rows written.
    /// </summary>
    public int RowCount { get; init; }
}