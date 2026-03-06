using Microsoft.ML;
using Microsoft.ML.Transforms;
using Valora.Domain.Entities;
using Valora.Infrastructure.ML.Models;

namespace Valora.Infrastructure.ML.Definitions;

/// <summary>
/// Price prediction category definition for laptops.
/// </summary>
public sealed class LaptopPriceModelDefinition
    : MlPricePredictionCategoryBase<LaptopProductFeatures, LaptopPriceTrainingRow, LaptopPricePrediction>
{
    /// <inheritdoc />
    public override string Key => "laptops";

    /// <inheritdoc />
    public override string DatasetFileName => "laptops.csv";

    /// <inheritdoc />
    public override string ModelFileName => "laptop-price-model.zip";

    /// <inheritdoc />
    public override IEstimator<ITransformer> BuildTrainingPipeline(MLContext mlContext)
    {
        string[] categoricalColumns =
        [
            nameof(LaptopPriceTrainingRow.Brand),
            nameof(LaptopPriceTrainingRow.Cpu),
            nameof(LaptopPriceTrainingRow.Gpu),
            nameof(LaptopPriceTrainingRow.Condition),
            nameof(LaptopPriceTrainingRow.Segment)
        ];

        string[] numericColumns =
        [
            nameof(LaptopPriceTrainingRow.RamGb),
            nameof(LaptopPriceTrainingRow.StorageGb),
            nameof(LaptopPriceTrainingRow.ScreenSizeInch),
            nameof(LaptopPriceTrainingRow.RefreshRate),
            nameof(LaptopPriceTrainingRow.ReleaseYear)
        ];

        IEstimator<ITransformer>? oneHotPipeline = null;

        foreach (string column in categoricalColumns)
        {
            OneHotEncodingEstimator? step = mlContext.Transforms.Categorical.OneHotEncoding(
                outputColumnName: $"{column}Encoded",
                inputColumnName: column);

            oneHotPipeline = oneHotPipeline is null
                ? step
                : oneHotPipeline.Append(step);
        }

        string[] featureColumns = categoricalColumns
            .Select(column => $"{column}Encoded")
            .Concat(numericColumns)
            .ToArray();

        if (oneHotPipeline is null)
        {
            throw new InvalidOperationException("No categorical columns were configured for laptop training.");
        }

        return oneHotPipeline
            .Append(mlContext.Transforms.Concatenate("Features", featureColumns))
            .Append(mlContext.Regression.Trainers.FastTree(
                labelColumnName: nameof(LaptopPriceTrainingRow.Price),
                featureColumnName: "Features"));
    }

    /// <inheritdoc />
    public override LaptopPriceTrainingRow MapFeaturesToTrainingRow(LaptopProductFeatures features)
    {
        ArgumentNullException.ThrowIfNull(features);

        return new LaptopPriceTrainingRow
        {
            Brand = features.Brand,
            Cpu = features.Cpu,
            RamGb = features.RamGb,
            StorageGb = features.StorageGb,
            Gpu = features.Gpu,
            ScreenSizeInch = features.ScreenSizeInch,
            RefreshRate = features.RefreshRate,
            ReleaseYear = features.ReleaseYear,
            Condition = features.Condition,
            Segment = features.Segment,
            Price = 0f
        };
    }

    /// <inheritdoc />
    public override LaptopProductFeatures CreateSanityFeatures()
    {
        return new LaptopProductFeatures
        {
            Brand = "Lenovo",
            Cpu = "Intel i7",
            RamGb = 16,
            StorageGb = 512,
            Gpu = "RTX 4060",
            ScreenSizeInch = 15.6f,
            RefreshRate = 144,
            ReleaseYear = 2023,
            Condition = "Refurbished",
            Segment = "Gaming"
        };
    }

    /// <inheritdoc />
    public override LaptopPriceTrainingRow CreateSanityTrainingRow()
        => MapFeaturesToTrainingRow(CreateSanityFeatures());
}