using Microsoft.ML;
using Microsoft.ML.Data;
using PriceWise.Infrastructure.ML.Abstractions;

namespace PriceWise.Infrastructure.ML.Training;

public sealed class TabularRegressionTrainer<TTrainingRow, TPrediction>
    where TTrainingRow : class, new()
    where TPrediction : class, new()
{
    private readonly MLContext _ml;
    private readonly ITabularRegressionDefinition<TTrainingRow, TPrediction> _definition;

    public TabularRegressionTrainer(
        MLContext ml,
        ITabularRegressionDefinition<TTrainingRow, TPrediction> definition
    )
    {
        _ml = ml;
        _definition = definition;
    }

    public RegressionTrainingResult TrainEvaluateAndSave(string csvPath, string modelPath)
    {
        if (!File.Exists(csvPath))
        {
            throw new FileNotFoundException($"Dataset not found at '{csvPath}'.", csvPath);
        }

        IDataView data = _ml.Data.LoadFromTextFile<TTrainingRow>(
            path: csvPath,
            hasHeader: true,
            separatorChar: _definition.SeparatorChar
        );

        DataOperationsCatalog.TrainTestData split = _ml.Data.TrainTestSplit(data, testFraction: 0.2, seed: 1);

        IEstimator<ITransformer> pipeline = _definition.BuildTrainingPipeline(_ml);

        ITransformer model = pipeline.Fit(split.TestSet);

        IDataView predictions = model.Transform(split.TestSet);
        RegressionMetrics metrics = _ml.Regression.Evaluate(predictions, labelColumnName: "Label");

        TTrainingRow? sanityInput = _definition.CreateSanitySample();
        IDataView sanityView = _ml.Data.LoadFromEnumerable(new[]
        {
            sanityInput
        });
        IDataView sanityScored = model.Transform(sanityView);

        float sanityPrediction = _ml.Data
            .CreateEnumerable<ScoreRow>(sanityScored, reuseRowObject: false)
            .First()
            .Score;

        Directory.CreateDirectory(Path.GetDirectoryName(modelPath)!);
        using FileStream fs = File.Create(modelPath);
        _ml.Model.Save(model, split.TrainSet.Schema, fs);

        int rowCount = _ml.Data.CreateEnumerable<TTrainingRow>(data, reuseRowObject: false).Count();

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
    
    private sealed class ScoreRow
    {
        public float Score { get; set; }
    }
}