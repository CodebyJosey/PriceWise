namespace Valora.Application.Abstractions.PricePrediction;

/// <summary>
/// Application service for training price prediction models.
/// </summary>
public interface IPriceTrainingService
{
    /// <summary>
    /// Trains a model for the specified category.
    /// </summary>
    /// <param name="categoryKey">The category key.</param>
    /// <returns>The training result.</returns>
    PriceTrainingResult Train(string categoryKey);
}