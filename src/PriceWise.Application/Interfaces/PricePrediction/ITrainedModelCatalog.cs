namespace PriceWise.Application.Abstractions.PricePrediction;

/// <summary>
/// Abstraction for accessing trained models by category.
/// The implementation may cache, reload and load models from disk.
/// </summary>
public interface ITrainedModelCatalog
{
    /// <summary>
    /// Gets the trained model wrapper for the specified category.
    /// </summary>
    /// <param name="categoryKey">The category key.</param>
    /// <returns>The trained model wrapper.</returns>
    object GetRequired(string categoryKey);

    /// <summary>
    /// Reloads the trained model for the specified category from disk.
    /// </summary>
    /// <param name="categoryKey">The category key.</param>
    void Reload(string categoryKey);
}