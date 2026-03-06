using Microsoft.ML;
using PriceWise.Application.Abstractions.PricePrediction;
using PriceWise.Infrastructure.ML.Abstractions;

namespace PriceWise.Infrastructure.ML.Definitions;

/// <summary>
/// Base class for strongly typed ML price prediction categories.
/// </summary>
/// <typeparam name="TFeatures">The input features type.</typeparam>
/// <typeparam name="TTrainingRow">The ML training row type.</typeparam>
/// <typeparam name="TPrediction">The ML prediction type.</typeparam>
public abstract class MlPricePredictionCategoryBase<TFeatures, TTrainingRow, TPrediction>
    : IMlPricePredictionCategory, IPricePredictionCategory<TFeatures, TTrainingRow, TPrediction>
    where TFeatures : class
    where TTrainingRow : class, new()
    where TPrediction : class, new()
{
    /// <inheritdoc />
    public abstract string Key { get; }

    /// <inheritdoc />
    public abstract string DatasetFileName { get; }

    /// <inheritdoc />
    public abstract string ModelFileName { get; }

    /// <inheritdoc />
    public Type FeaturesType => typeof(TFeatures);

    /// <inheritdoc />
    public Type TrainingRowType => typeof(TTrainingRow);

    /// <inheritdoc />
    public Type PredictionType => typeof(TPrediction);

    /// <inheritdoc />
    public abstract IEstimator<ITransformer> BuildTrainingPipeline(MLContext mlContext);

    /// <inheritdoc />
    public abstract TTrainingRow MapFeaturesToTrainingRow(TFeatures features);

    /// <inheritdoc />
    public abstract TFeatures CreateSanityFeatures();

    /// <inheritdoc />
    public abstract TTrainingRow CreateSanityTrainingRow();

    object IPricePredictionCategory.MapFeaturesToTrainingRow(object features)
    {
        ArgumentNullException.ThrowIfNull(features);

        if (features is not TFeatures typedFeatures)
        {
            throw new ArgumentException(
                $"Invalid features type for category '{Key}'. Expected '{typeof(TFeatures).Name}', got '{features.GetType().Name}'.",
                nameof(features));
        }

        return MapFeaturesToTrainingRow(typedFeatures);
    }

    object IPricePredictionCategory.CreateSanityFeatures()
        => CreateSanityFeatures();

    object IPricePredictionCategory.CreateSanityTrainingRow()
        => CreateSanityTrainingRow();
}