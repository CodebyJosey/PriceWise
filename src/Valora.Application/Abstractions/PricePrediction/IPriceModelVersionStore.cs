namespace Valora.Application.Abstractions.PricePrediction;

/// <summary>
/// Stores and retrieves versioned model metadata per category.
/// </summary>
public interface IPriceModelVersionStore
{
    /// <summary>
    /// Gets the next version number for the specified category.
    /// </summary>
    /// <param name="categoryKey">The category key.</param>
    /// <returns>The next version number.</returns>
    int GetNextVersionNumber(string categoryKey);

    /// <summary>
    /// Saves a model version and marks it as active.
    /// </summary>
    /// <param name="metadata">The metadata to save.</param>
    void SaveVersion(PriceTrainingMetadata metadata);

    /// <summary>
    /// Attempts to get the active model metadata for a category.
    /// </summary>
    /// <param name="categoryKey">The category key.</param>
    /// <param name="metadata">The resolved metadata.</param>
    /// <returns>True when found; otherwise false.</returns>
    bool TryGetActive(string categoryKey, out PriceTrainingMetadata? metadata);

    /// <summary>
    /// Attempts to get a specific model version for a category.
    /// </summary>
    /// <param name="categoryKey">The category key.</param>
    /// <param name="version">The version number.</param>
    /// <param name="metadata">The resolved metadata.</param>
    /// <returns>True when found; otherwise false.</returns>
    bool TryGetVersion(string categoryKey, int version, out PriceTrainingMetadata? metadata);

    /// <summary>
    /// Gets all known versions for a category.
    /// </summary>
    /// <param name="categoryKey">The category key.</param>
    /// <returns>The versions ordered descending by version number.</returns>
    IReadOnlyList<PriceTrainingMetadata> GetAllVersions(string categoryKey);

    /// <summary>
    /// Sets the active version for a category.
    /// </summary>
    /// <param name="categoryKey">The category key.</param>
    /// <param name="version">The version number to activate.</param>
    /// <returns>True when successful; otherwise false.</returns>
    bool SetActive(string categoryKey, int version);
}