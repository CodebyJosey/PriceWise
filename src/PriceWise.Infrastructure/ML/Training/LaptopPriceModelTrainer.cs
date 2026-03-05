using System.Diagnostics;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Trainers.FastTree;
using PriceWise.Infrastructure.ML.Models;

namespace PriceWise.Infrastructure.ML.Training;

/// <summary>
/// Trains, evaluates, and saves a regression model for laptop price prediction.
/// </summary>
public sealed class LaptopPriceModelTrainer
{
    private readonly MLContext _ml = new(seed: 1);

    public RegressionTrainingResult TrainEvaluateAndSave(string csvPath, string modelPath)
    {
        Stopwatch? swTotal = Stopwatch.StartNew();
        Stopwatch? stopwatch = Stopwatch.StartNew();

        if (!File.Exists(csvPath))
        {
            throw new FileNotFoundException($"Dataset not found at '{csvPath}'.", csvPath);
        }

        // ✅ IMPORTANT: Typed load, so LoadColumn/Label mapping is actually applied.
        IDataView data = _ml.Data.LoadFromTextFile<LaptopPriceTrainingRow>(
            path: csvPath,
            hasHeader: true,
            separatorChar: ','
        );
        Console.WriteLine($"[TIMER] LoadFromTextFile: {stopwatch.ElapsedMilliseconds} ms");
        stopwatch.Restart();

        List<LaptopPriceTrainingRow>? rows = _ml.Data.CreateEnumerable<LaptopPriceTrainingRow>(data, reuseRowObject: false).ToList();
        if (rows.Count == 0)
        {
            throw new InvalidOperationException("Dataset is empty after loading.");
        }

        float min = rows.Min(r => r.Price);
        float max = rows.Max(r => r.Price);
        float avg = rows.Average(r => r.Price);
        int zeros = rows.Count(r => r.Price == 0);

        if (min == 0 && max == 0)
        {
            throw new InvalidOperationException("All labels (Price) are 0. This usually means the Price column isn't parsed correctly (culture/format/column mismatch).");
        }

        Console.WriteLine($"[DATA CHECK] rows={rows.Count}, price: min={min}, max={max}, avg={avg}, zeros={zeros}");

        DataOperationsCatalog.TrainTestData split = _ml.Data.TrainTestSplit(data, testFraction: 0.2, seed: 1);
        Console.WriteLine($"[TIMER] TrainTestSplit: {stopwatch.ElapsedMilliseconds} ms");
        stopwatch.Restart();

        // String -> Key -> OneHot is fine.
        EstimatorChain<RegressionPredictionTransformer<LinearRegressionModelParameters>>? pipeline =
            _ml.Transforms.Text.NormalizeText("BrandNorm", nameof(LaptopPriceTrainingRow.Brand))
            .Append(_ml.Transforms.Text.NormalizeText("CpuNorm", nameof(LaptopPriceTrainingRow.Cpu)))
            .Append(_ml.Transforms.Text.NormalizeText("GpuNorm", nameof(LaptopPriceTrainingRow.Gpu)))

            .Append(_ml.Transforms.Conversion.MapValueToKey("BrandKey", "BrandNorm"))
            .Append(_ml.Transforms.Categorical.OneHotEncoding("BrandOneHot", "BrandKey"))

            .Append(_ml.Transforms.Conversion.MapValueToKey("CpuKey", "CpuNorm"))
            .Append(_ml.Transforms.Categorical.OneHotEncoding("CpuOneHot", "CpuKey"))

            .Append(_ml.Transforms.Conversion.MapValueToKey("GpuKey", "GpuNorm"))
            .Append(_ml.Transforms.Categorical.OneHotEncoding("GpuOneHot", "GpuKey"))

            .Append(_ml.Transforms.Concatenate("Features",
                "BrandOneHot", "CpuOneHot", "GpuOneHot",
                nameof(LaptopPriceTrainingRow.RamGb),
                nameof(LaptopPriceTrainingRow.StorageGb)))

            .Append(_ml.Regression.Trainers.Sdca(labelColumnName: "Label", featureColumnName: "Features"));

        // ✅ Fit on train set only
        ITransformer model = pipeline.Fit(split.TrainSet);
        Console.WriteLine($"[TIMER] Fit: {stopwatch.ElapsedMilliseconds} ms");
        stopwatch.Restart();

        // Evaluate on test set
        IDataView testPredictions = model.Transform(split.TestSet);
        RegressionMetrics metrics = _ml.Regression.Evaluate(testPredictions, labelColumnName: "Label");
        Console.WriteLine($"[TIMER] Evaluate: {stopwatch.ElapsedMilliseconds} ms");
        stopwatch.Stop();

        Console.WriteLine($"[TIMER] Evaluate: {swTotal.ElapsedMilliseconds} ms");

        // Quick sanity prediction (should NOT be 0 if your labels are 599..1199 etc.)
        PredictionEngine<LaptopPriceTrainingRow, LaptopPricePrediction>? sanityEngine = _ml.Model.CreatePredictionEngine<LaptopPriceTrainingRow, LaptopPricePrediction>(model);
        LaptopPriceTrainingRow? sanityRow = new LaptopPriceTrainingRow
        {
            Brand = "Dell",
            Cpu = "i7",
            RamGb = 16,
            StorageGb = 512,
            Gpu = "RTX3050",
            Price = 0
        };
        float sanityPrediction = sanityEngine.Predict(sanityRow).PredictedPrice;

        Directory.CreateDirectory(Path.GetDirectoryName(modelPath)!);
        using (FileStream fs = File.Create(modelPath))
        {
            _ml.Model.Save(model, split.TrainSet.Schema, fs);
        }

        int rowCount = _ml.Data.CreateEnumerable<LaptopPriceTrainingRow>(data, reuseRowObject: false).Count();

        double r2 = metrics.RSquared;
        double? r2Safe = double.IsNaN(r2) || double.IsInfinity(r2) ? null : r2;

        return new RegressionTrainingResult(
            Rmse: metrics.RootMeanSquaredError,
            RSquared: r2Safe,
            RowCount: rowCount,
            ModelPath: modelPath,
            SanityPrediction: sanityPrediction
        );
    }
}