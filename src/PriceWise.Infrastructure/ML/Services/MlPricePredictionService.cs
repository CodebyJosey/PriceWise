using System.Reflection;
using Microsoft.ML;
using PriceWise.Application.Abstractions.PricePrediction;
using PriceWise.Infrastructure.ML.Models;

namespace PriceWise.Infrastructure.ML.Services;

/// <summary>
/// Generic ML.NET price prediction service that works per category.
/// </summary>
public sealed class MlPricePredictionService : IPricePredictionService
{
    private readonly IPricePredictionCategoryRegistry _categoryRegistry;
    private readonly ITrainedModelCatalog _modelCatalog;

    /// <summary>
    /// Initializes a new instance of the <see cref="MlPricePredictionService"/> class.
    /// </summary>
    /// <param name="categoryRegistry">The category registry.</param>
    /// <param name="modelCatalog">The model catalog.</param>
    public MlPricePredictionService(
        IPricePredictionCategoryRegistry categoryRegistry,
        ITrainedModelCatalog modelCatalog)
    {
        _categoryRegistry = categoryRegistry;
        _modelCatalog = modelCatalog;
    }

    /// <inheritdoc />
    public object Predict(string categoryKey, object features)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryKey);
        ArgumentNullException.ThrowIfNull(features);

        IPricePredictionCategory category = _categoryRegistry.GetRequired(categoryKey);
        object trainingRow = category.MapFeaturesToTrainingRow(features);

        var trainedModel = (TrainedModel)_modelCatalog.GetRequired(categoryKey);

        return PredictInternal(trainedModel, trainingRow);
    }

    /// <inheritdoc />
    public object SanityPredict(string categoryKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryKey);

        IPricePredictionCategory category = _categoryRegistry.GetRequired(categoryKey);
        object trainingRow = category.CreateSanityTrainingRow();

        var trainedModel = (TrainedModel)_modelCatalog.GetRequired(categoryKey);

        return PredictInternal(trainedModel, trainingRow);
    }

    private static object PredictInternal(TrainedModel trainedModel, object trainingRow)
    {
        MethodInfo? createEngineMethod = typeof(PredictionEngineFactory)
            .GetMethod(nameof(PredictionEngineFactory.Create), BindingFlags.Public | BindingFlags.Static);

        if (createEngineMethod is null)
        {
            throw new InvalidOperationException("Could not find PredictionEngineFactory.Create method.");
        }

        MethodInfo genericMethod = createEngineMethod.MakeGenericMethod(
            trainedModel.TrainingRowType,
            trainedModel.PredictionType);

        object? engine = genericMethod.Invoke(null, [trainedModel.MlContext, trainedModel.Transformer]);

        if (engine is null)
        {
            throw new InvalidOperationException("Could not create prediction engine.");
        }

        MethodInfo? predictMethod = engine.GetType().GetMethod("Predict", [trainedModel.TrainingRowType]);

        if (predictMethod is null)
        {
            throw new InvalidOperationException("Could not find Predict method on prediction engine.");
        }

        object? prediction = predictMethod.Invoke(engine, [trainingRow]);

        if (prediction is null)
        {
            throw new InvalidOperationException("Prediction returned null.");
        }

        return prediction;
    }

    private static class PredictionEngineFactory
    {
        public static PredictionEngine<TInput, TOutput> Create<TInput, TOutput>(
            MLContext mlContext,
            ITransformer transformer)
            where TInput : class
            where TOutput : class, new()
        {
            return mlContext.Model.CreatePredictionEngine<TInput, TOutput>(transformer);
        }
    }
}