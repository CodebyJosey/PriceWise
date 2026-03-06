using Microsoft.ML;
using Microsoft.ML.Transforms;
using PriceWise.Domain.Entities;
using PriceWise.Infrastructure.ML.Models;

namespace PriceWise.Infrastructure.ML.Definitions;

/// <summary>
/// Price prediction category definition for tablets.
/// </summary>
public sealed class TabletPriceModelDefinition
    : MlPricePredictionCategoryBase<TabletProductFeatures, TabletPriceTrainingRow, TabletPricePrediction>
{
    /// <inheritdoc />
    public override string Key => "tablets";

    /// <inheritdoc />
    public override string DatasetFileName => "tablets.csv";

    /// <inheritdoc />
    public override string ModelFileName => "tablet-price-model.zip";

    /// <inheritdoc />
    public override IEstimator<ITransformer> BuildTrainingPipeline(MLContext mlContext)
    {
        string[] categoricalColumns =
        [
            nameof(TabletPriceTrainingRow.Brand),
            nameof(TabletPriceTrainingRow.ModelFamily),
            nameof(TabletPriceTrainingRow.Condition),
            nameof(TabletPriceTrainingRow.Connectivity)
        ];

        string[] numericColumns =
        [
            nameof(TabletPriceTrainingRow.StorageGb),
            nameof(TabletPriceTrainingRow.RamGb),
            nameof(TabletPriceTrainingRow.ScreenSizeInch),
            nameof(TabletPriceTrainingRow.ReleaseYear)
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
            throw new InvalidOperationException("No categorical columns were configured for tablet training.");
        }

        return oneHotPipeline
            .Append(mlContext.Transforms.Concatenate("Features", featureColumns))
            .Append(mlContext.Regression.Trainers.FastTree(
                labelColumnName: nameof(TabletPriceTrainingRow.Price),
                featureColumnName: "Features"));
    }

    /// <inheritdoc />
    public override TabletPriceTrainingRow MapFeaturesToTrainingRow(TabletProductFeatures features)
    {
        ArgumentNullException.ThrowIfNull(features);

        return new TabletPriceTrainingRow
        {
            Brand = features.Brand,
            ModelFamily = features.ModelFamily,
            StorageGb = features.StorageGb,
            RamGb = features.RamGb,
            ScreenSizeInch = features.ScreenSizeInch,
            Condition = features.Condition,
            ReleaseYear = features.ReleaseYear,
            Connectivity = features.Connectivity,
            Price = 0f
        };
    }

    /// <inheritdoc />
    public override TabletProductFeatures CreateSanityFeatures()
    {
        return new TabletProductFeatures
        {
            Brand = "Apple",
            ModelFamily = "iPad Air",
            StorageGb = 256,
            RamGb = 8,
            ScreenSizeInch = 10.9f,
            Condition = "Refurbished",
            ReleaseYear = 2023,
            Connectivity = "WiFi"
        };
    }

    /// <inheritdoc />
    public override TabletPriceTrainingRow CreateSanityTrainingRow()
        => MapFeaturesToTrainingRow(CreateSanityFeatures());
}