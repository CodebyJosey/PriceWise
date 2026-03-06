using Microsoft.ML;

namespace PriceWise.Infrastructure.ML.Abstractions;

public interface ITabularRegressionDefinition<TTrainingRow, TPrediction>
    where TTrainingRow : class, new()
    where TPrediction : class, new()
{
    string ModelName { get; }
    char SeparatorChar => ',';
    IEstimator<ITransformer> BuildTrainingPipeline(MLContext ml);
    TTrainingRow CreateSanitySample();
}