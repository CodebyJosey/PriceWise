namespace Valora.Application.Abstractions.PricePrediction;

/// <summary>
/// Represents persisted metadata for a trained model version of a category.
/// </summary>
public sealed class PriceTrainingMetadata
{
    /// <summary>
    /// Gets or sets the category key.
    /// </summary>
    public required string CategoryKey { get; init; }

    /// <summary>
    /// Gets or sets the version number.
    /// </summary>
    public int Version { get; init; }

    /// <summary>
    /// Gets or sets the UTC timestamp of the training run.
    /// </summary>
    public DateTime TrainedAtUtc { get; init; }

    /// <summary>
    /// Gets or sets the dataset path used for training.
    /// </summary>
    public required string DatasetPath { get; init; }

    /// <summary>
    /// Gets or sets the model path where the trained model version was saved.
    /// </summary>
    public required string ModelPath { get; init; }

    /// <summary>
    /// Gets or sets the R-squared metric.
    /// </summary>
    public double RSquared { get; init; }

    /// <summary>
    /// Gets or sets the root mean squared error metric.
    /// </summary>
    public double RootMeanSquaredError { get; init; }

    /// <summary>
    /// Gets or sets the mean absolute error metric.
    /// </summary>
    public double MeanAbsoluteError { get; init; }

    /// <summary>
    /// Gets or sets the mean squared error metric.
    /// </summary>
    public double MeanSquaredError { get; init; }
}