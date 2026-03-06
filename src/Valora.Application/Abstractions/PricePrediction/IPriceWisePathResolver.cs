namespace Valora.Application.Abstractions.PricePrediction;

/// <summary>
/// Resolves standard file system paths for datasets, versioned models and versioned metadata.
/// </summary>
public interface IValoraPathResolver
{
    /// <summary>
    /// Gets the dataset path for the specified category.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <returns>The dataset path.</returns>
    string GetDatasetPath(IPricePredictionCategory category);

    /// <summary>
    /// Gets the model directory path for the specified category.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <returns>The model directory path.</returns>
    string GetModelDirectoryPath(IPricePredictionCategory category);

    /// <summary>
    /// Gets the metadata directory path for the specified category.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <returns>The metadata directory path.</returns>
    string GetMetadataDirectoryPath(IPricePredictionCategory category);

    /// <summary>
    /// Gets the versioned model file path for the specified category and version.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="version">The version number.</param>
    /// <returns>The versioned model file path.</returns>
    string GetVersionedModelPath(IPricePredictionCategory category, int version);

    /// <summary>
    /// Gets the versioned metadata file path for the specified category and version.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="version">The version number.</param>
    /// <returns>The versioned metadata file path.</returns>
    string GetVersionedMetadataPath(IPricePredictionCategory category, int version);

    /// <summary>
    /// Gets the active metadata file path for the specified category.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <returns>The active metadata file path.</returns>
    string GetActiveMetadataPath(IPricePredictionCategory category);
}