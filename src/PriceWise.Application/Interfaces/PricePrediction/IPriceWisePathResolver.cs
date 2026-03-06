namespace PriceWise.Application.Abstractions.PricePrediction;

/// <summary>
/// Resolves standard file system paths for datasets and trained models.
/// </summary>
public interface IPriceWisePathResolver
{
    /// <summary>
    /// Gets the dataset path for the specified category.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <returns>The dataset path.</returns>
    string GetDatasetPath(IPricePredictionCategory category);

    /// <summary>
    /// Gets the model path for the specified category.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <returns>The model path.</returns>
    string GetModelPath(IPricePredictionCategory category);
}