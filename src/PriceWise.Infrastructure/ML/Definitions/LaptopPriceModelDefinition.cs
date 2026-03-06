using Microsoft.ML;
using PriceWise.Infrastructure.ML.Abstractions;
using PriceWise.Infrastructure.ML.Models;

namespace PriceWise.Infrastructure.ML.Definitions;

public sealed class LaptopPriceModelDefinition
    :ITabularRegressionDefinition<LaptopPriceTrainingRow, LaptopPricePrediction>
{
    public string ModelName => "laptop-price";

    public IEstimator<ITransformer> BuildTrainingPipeline(MLContext ml)
    {
        return ml.Transforms.Text.NormalizeText("BrandNorm", nameof(LaptopPriceTrainingRow.Brand))
            .Append(ml.Transforms.Text.NormalizeText("CpuNorm", nameof(LaptopPriceTrainingRow.Cpu)))
            .Append(ml.Transforms.Text.NormalizeText("GpuNorm", nameof(LaptopPriceTrainingRow.Gpu)))

            .Append(ml.Transforms.Conversion.MapValueToKey("BrandKey", "BrandNorm"))
            .Append(ml.Transforms.Categorical.OneHotEncoding("BrandVec", "BrandKey"))

            .Append(ml.Transforms.Conversion.MapValueToKey("CpuKey", "CpuNorm"))
            .Append(ml.Transforms.Categorical.OneHotEncoding("CpuVec", "CpuKey"))

            .Append(ml.Transforms.Conversion.MapValueToKey("GpuKey", "GpuNorm"))
            .Append(ml.Transforms.Categorical.OneHotEncoding("GpuVec", "GpuKey"))

            .Append(ml.Transforms.Concatenate("Features",
                "BrandVec",
                "CpuVec",
                "GpuVec",
                nameof(LaptopPriceTrainingRow.RamGb),
                nameof(LaptopPriceTrainingRow.StorageGb)))

            .Append(ml.Regression.Trainers.FastTree(
                labelColumnName: "Label",
                featureColumnName: "Features",
                numberOfLeaves: 20,
                numberOfTrees: 100,
                minimumExampleCountPerLeaf: 2
            ));
    }

    public LaptopPriceTrainingRow CreateSanitySample()
    {
        return new LaptopPriceTrainingRow
        {
            Brand = "Dell",
            Cpu = "i7",
            RamGb = 16,
            StorageGb = 512,
            Gpu = "RTX3050",
            Price = 0
        };
    }
}