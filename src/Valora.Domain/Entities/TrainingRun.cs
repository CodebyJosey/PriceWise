namespace Valora.Domain.Entities;

/// <summary>
/// Represents a training run for a model category.
/// </summary>
public class TrainingRun
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the category key.
    /// </summary>
    public string CategoryKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the created model version.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Gets or sets the dataset path.
    /// </summary>
    public string DatasetPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the model path.
    /// </summary>
    public string ModelPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the R-squared metric.
    /// </summary>
    public double RSquared { get; set; }

    /// <summary>
    /// Gets or sets the RMSE metric.
    /// </summary>
    public double RootMeanSquaredError { get; set; }

    /// <summary>
    /// Gets or sets the MAE metric.
    /// </summary>
    public double MeanAbsoluteError { get; set; }

    /// <summary>
    /// Gets or sets the MSE metric.
    /// </summary>
    public double MeanSquaredError { get; set; }

    /// <summary>
    /// Gets or sets the UTC creation timestamp.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}