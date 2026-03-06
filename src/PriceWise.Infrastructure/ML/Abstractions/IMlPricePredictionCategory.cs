using Microsoft.ML;
using PriceWise.Application.Abstractions.PricePrediction;

namespace PriceWise.Infrastructure.ML.Abstractions;

/// <summary>
/// ML-specific extension of a price prediction category.
/// This contract stays inside Infrastructure so Application remains ML-agnostic.
/// </summary>
public interface IMlPricePredictionCategory : IPricePredictionCategory
{
    /// <summary>
    /// Builds the ML.NET training pipeline for this category.
    /// </summary>
    /// <param name="mlContext">The ML context.</param>
    /// <returns>The estimator pipeline.</returns>
    IEstimator<ITransformer> BuildTrainingPipeline(MLContext mlContext);
}