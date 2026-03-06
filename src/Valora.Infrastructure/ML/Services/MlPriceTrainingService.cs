using System.Reflection;
using Microsoft.ML;
using Microsoft.ML.Data;
using Valora.Application.Abstractions.PricePrediction;
using Valora.Infrastructure.ML.Abstractions;

namespace Valora.Infrastructure.ML.Services;

/// <summary>
/// Generic ML.NET training service that works for all registered categories.
/// Creates a new version on every successful training run.
/// </summary>
public sealed class MlPriceTrainingService : IPriceTrainingService
{
    private readonly IPricePredictionCategoryRegistry _categoryRegistry;
    private readonly IValoraPathResolver _pathResolver;
    private readonly ITrainedModelCatalog _trainedModelCatalog;
    private readonly IPriceModelVersionStore _versionStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="MlPriceTrainingService"/> class.
    /// </summary>
    /// <param name="categoryRegistry">The category registry.</param>
    /// <param name="pathResolver">The path resolver.</param>
    /// <param name="trainedModelCatalog">The trained model catalog.</param>
    /// <param name="versionStore">The version store.</param>
    public MlPriceTrainingService(
        IPricePredictionCategoryRegistry categoryRegistry,
        IValoraPathResolver pathResolver,
        ITrainedModelCatalog trainedModelCatalog,
        IPriceModelVersionStore versionStore)
    {
        _categoryRegistry = categoryRegistry;
        _pathResolver = pathResolver;
        _trainedModelCatalog = trainedModelCatalog;
        _versionStore = versionStore;
    }

    /// <inheritdoc />
    public PriceTrainingResult Train(string categoryKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryKey);

        IPricePredictionCategory baseCategory = _categoryRegistry.GetRequired(categoryKey);

        if (baseCategory is not IMlPricePredictionCategory category)
        {
            throw new InvalidOperationException(
                $"Category '{categoryKey}' is not an ML-enabled category.");
        }

        string datasetPath = _pathResolver.GetDatasetPath(category);

        if (!File.Exists(datasetPath))
        {
            throw new FileNotFoundException(
                $"Dataset file was not found for category '{categoryKey}'. Expected path: '{datasetPath}'.",
                datasetPath);
        }

        int version = _versionStore.GetNextVersionNumber(categoryKey);
        string modelPath = _pathResolver.GetVersionedModelPath(category, version);

        string? modelDirectory = Path.GetDirectoryName(modelPath);
        if (!string.IsNullOrWhiteSpace(modelDirectory))
        {
            Directory.CreateDirectory(modelDirectory);
        }

        MLContext mlContext = new(seed: 1);

        IDataView data = LoadDataView(mlContext, datasetPath, category.TrainingRowType);

        EnsureRequiredColumnsExist(data.Schema, category);

        IEstimator<ITransformer> pipeline = category.BuildTrainingPipeline(mlContext);

        DataOperationsCatalog.TrainTestData split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

        ITransformer model = pipeline.Fit(split.TrainSet);

        IDataView predictions = model.Transform(split.TestSet);

        EnsureColumnExists(
            predictions.Schema,
            "Price",
            "The transformed prediction schema does not contain the required label column 'Price'.");

        EnsureColumnExists(
            predictions.Schema,
            "Score",
            "The transformed prediction schema does not contain the required score column 'Score'.");

        RegressionMetrics metrics = mlContext.Regression.Evaluate(
            predictions,
            labelColumnName: "Price",
            scoreColumnName: "Score");

        SaveModel(mlContext, model, split.TrainSet.Schema, modelPath);

        PriceTrainingMetadata metadata = new()
        {
            CategoryKey = category.Key,
            Version = version,
            TrainedAtUtc = DateTime.UtcNow,
            DatasetPath = datasetPath,
            ModelPath = modelPath,
            RSquared = metrics.RSquared,
            RootMeanSquaredError = metrics.RootMeanSquaredError,
            MeanAbsoluteError = metrics.MeanAbsoluteError,
            MeanSquaredError = metrics.MeanSquaredError
        };

        _versionStore.SaveVersion(metadata);
        _trainedModelCatalog.Reload(categoryKey);

        return new PriceTrainingResult
        {
            CategoryKey = category.Key,
            Version = version,
            DatasetPath = datasetPath,
            ModelPath = modelPath,
            RSquared = metrics.RSquared,
            RootMeanSquaredError = metrics.RootMeanSquaredError,
            MeanAbsoluteError = metrics.MeanAbsoluteError,
            MeanSquaredError = metrics.MeanSquaredError
        };
    }

    private static void SaveModel(
        MLContext mlContext,
        ITransformer model,
        DataViewSchema schema,
        string modelPath)
    {
        using FileStream fileStream = File.Create(modelPath);
        mlContext.Model.Save(model, schema, fileStream);
    }

    private static IDataView LoadDataView(MLContext mlContext, string datasetPath, Type rowType)
    {
        MethodInfo? helperMethod = typeof(MlPriceTrainingService)
            .GetMethod(nameof(LoadDataViewGeneric), BindingFlags.NonPublic | BindingFlags.Static);

        if (helperMethod is null)
        {
            throw new InvalidOperationException("Could not find LoadDataViewGeneric helper method.");
        }

        MethodInfo genericMethod = helperMethod.MakeGenericMethod(rowType);

        object? result = genericMethod.Invoke(null, new object[] { mlContext, datasetPath });

        if (result is not IDataView dataView)
        {
            throw new InvalidOperationException("Could not load dataset as IDataView.");
        }

        return dataView;
    }

    private static IDataView LoadDataViewGeneric<TRow>(MLContext mlContext, string datasetPath)
        where TRow : class
    {
        return mlContext.Data.LoadFromTextFile<TRow>(
            path: datasetPath,
            hasHeader: true,
            separatorChar: ',',
            allowQuoting: true,
            trimWhitespace: true);
    }

    private static void EnsureRequiredColumnsExist(DataViewSchema schema, IPricePredictionCategory category)
    {
        string[] requiredColumns = category.TrainingRowType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(property => property.Name)
            .ToArray();

        List<string> missingColumns = new();

        foreach (string columnName in requiredColumns)
        {
            if (schema.GetColumnOrNull(columnName) is null)
            {
                missingColumns.Add(columnName);
            }
        }

        if (missingColumns.Count == 0)
        {
            return;
        }

        string availableColumns = string.Join(", ", schema.Select(column => column.Name));

        throw new ArgumentException(
            $"The loaded dataset schema for category '{category.Key}' is missing required columns: {string.Join(", ", missingColumns)}. " +
            $"Available columns: {availableColumns}");
    }

    private static void EnsureColumnExists(DataViewSchema schema, string columnName, string errorMessage)
    {
        if (schema.GetColumnOrNull(columnName) is not null)
        {
            return;
        }

        string availableColumns = string.Join(", ", schema.Select(column => column.Name));

        throw new ArgumentException($"{errorMessage} Available columns: {availableColumns}");
    }
}