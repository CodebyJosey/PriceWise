namespace PriceWise.Application.Abstractions.PricePrediction;

/// <summary>
/// Application service for running price predictions.
/// </summary>
public interface IPricePredictionService
{
    /// <summary>
    /// Predicts a price using the specified category and input features.
    /// </summary>
    /// <param name="categoryKey">The category key.</param>
    /// <param name="features">The input features object.</param>
    /// <returns>The raw prediction object.</returns>
    object Predict(string categoryKey, object features);

    /// <summary>
    /// Runs a sanity prediction using the category's own sample data.
    /// </summary>
    /// <param name="categoryKey">The category key.</param>
    /// <returns>The raw prediction object.</returns>
    object SanityPredict(string categoryKey);
}