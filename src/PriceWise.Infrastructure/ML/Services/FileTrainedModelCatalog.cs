using Microsoft.ML;
using PriceWise.Application.Abstractions.PricePrediction;
using PriceWise.Infrastructure.ML.Models;

namespace PriceWise.Infrastructure.ML.Services;

/// <summary>
/// Loads and caches trained models from disk per category.
/// </summary>
public sealed class FileTrainedModelCatalog : ITrainedModelCatalog
{
    private readonly IPricePredictionCategoryRegistry _categoryRegistry;
    private readonly IPriceWisePathResolver _pathResolver;
    private readonly object _syncRoot = new();
    private readonly Dictionary<string, TrainedModel> _cache = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="FileTrainedModelCatalog"/> class.
    /// </summary>
    /// <param name="categoryRegistry">The category registry.</param>
    /// <param name="pathResolver">The path resolver.</param>
    public FileTrainedModelCatalog(
        IPricePredictionCategoryRegistry categoryRegistry,
        IPriceWisePathResolver pathResolver)
    {
        _categoryRegistry = categoryRegistry;
        _pathResolver = pathResolver;
    }

    /// <inheritdoc />
    public object GetRequired(string categoryKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryKey);

        lock (_syncRoot)
        {
            if (_cache.TryGetValue(categoryKey, out TrainedModel? cached))
            {
                return cached;
            }

            TrainedModel model = Load(categoryKey);
            _cache[categoryKey] = model;
            return model;
        }
    }

    /// <inheritdoc />
    public void Reload(string categoryKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryKey);

        lock (_syncRoot)
        {
            TrainedModel model = Load(categoryKey);
            _cache[categoryKey] = model;
        }
    }

    private TrainedModel Load(string categoryKey)
    {
        var category = _categoryRegistry.GetRequired(categoryKey);
        string modelPath = _pathResolver.GetModelPath(category);

        if (!File.Exists(modelPath))
        {
            throw new FileNotFoundException(
                $"No trained model file was found for category '{categoryKey}'. Expected path: '{modelPath}'.",
                modelPath);
        }

        MLContext mlContext = new(seed: 1);

        using FileStream stream = File.OpenRead(modelPath);
        ITransformer transformer = mlContext.Model.Load(stream, out _);

        return new TrainedModel
        {
            MlContext = mlContext,
            Transformer = transformer,
            TrainingRowType = category.TrainingRowType,
            PredictionType = category.PredictionType
        };
    }
}