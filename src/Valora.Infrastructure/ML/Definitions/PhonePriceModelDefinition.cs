using Microsoft.ML;
using Microsoft.ML.Transforms;
using Valora.Domain.Entities;
using Valora.Infrastructure.ML.Models;

namespace Valora.Infrastructure.ML.Definitions;

/// <summary>
/// Price prediction category definition for phones.
/// </summary>
public sealed class PhonePriceModelDefinition
    : MlPricePredictionCategoryBase<PhoneProductFeatures, PhonePriceTrainingRow, PhonePricePrediction>
{
    /// <inheritdoc />
    public override string Key => "phones";

    /// <inheritdoc />
    public override string DatasetFileName => "phones.csv";

    /// <inheritdoc />
    public override string ModelFileName => "phone-price-model.zip";

    /// <inheritdoc />
    public override IEstimator<ITransformer> BuildTrainingPipeline(MLContext mlContext)
    {
        string[] categoricalColumns =
        [
            nameof(PhonePriceTrainingRow.Brand),
            nameof(PhonePriceTrainingRow.ModelFamily),
            nameof(PhonePriceTrainingRow.Condition)
        ];

        string[] numericColumns =
        [
            nameof(PhonePriceTrainingRow.StorageGb),
            nameof(PhonePriceTrainingRow.RamGb),
            nameof(PhonePriceTrainingRow.BatteryHealth),
            nameof(PhonePriceTrainingRow.ReleaseYear)
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
            throw new InvalidOperationException("No categorical columns were configured for phone training.");
        }

        return oneHotPipeline
            .Append(mlContext.Transforms.Concatenate("Features", featureColumns))
            .Append(mlContext.Regression.Trainers.FastTree(
                labelColumnName: nameof(PhonePriceTrainingRow.Price),
                featureColumnName: "Features"));
    }

    /// <inheritdoc />
    public override PhonePriceTrainingRow MapFeaturesToTrainingRow(PhoneProductFeatures features)
    {
        ArgumentNullException.ThrowIfNull(features);

        return new PhonePriceTrainingRow
        {
            Brand = features.Brand,
            ModelFamily = features.ModelFamily,
            StorageGb = features.StorageGb,
            RamGb = features.RamGb,
            BatteryHealth = features.BatteryHealth,
            Condition = features.Condition,
            ReleaseYear = features.ReleaseYear,
            Price = 0f
        };
    }

    /// <inheritdoc />
    public override PhoneProductFeatures CreateSanityFeatures()
    {
        return new PhoneProductFeatures
        {
            Brand = "Apple",
            ModelFamily = "iPhone 13",
            StorageGb = 128,
            RamGb = 4,
            BatteryHealth = 88,
            Condition = "Used",
            ReleaseYear = 2021
        };
    }

    /// <inheritdoc />
    public override PhonePriceTrainingRow CreateSanityTrainingRow()
        => MapFeaturesToTrainingRow(CreateSanityFeatures());
}