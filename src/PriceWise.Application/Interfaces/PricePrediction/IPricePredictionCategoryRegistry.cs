namespace PriceWise.Application.Abstractions.PricePrediction;

/// <summary>
/// Registry for all available price prediction categories.
/// </summary>
public interface IPricePredictionCategoryRegistry
{
    /// <summary>
    /// Gets all registered categories.
    /// </summary>
    /// <returns>All registered categories.</returns>
    IReadOnlyCollection<IPricePredictionCategory> GetAll();

    /// <summary>
    /// Gets a category by key.
    /// Throws an exception when the category does not exist.
    /// </summary>
    /// <param name="key">The category key.</param>
    /// <returns>The category.</returns>
    IPricePredictionCategory GetRequired(string key);

    /// <summary>
    /// Attempts to get a category by key.
    /// </summary>
    /// <param name="key">The category key.</param>
    /// <param name="category">The resolved category, when found.</param>
    /// <returns>True when found; otherwise false.</returns>
    bool TryGet(string key, out IPricePredictionCategory? category);
}