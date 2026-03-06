using Microsoft.ML;

namespace Valora.Infrastructure.ML.Models;

/// <summary>
/// Wraps a loaded ML.NET model and its prediction engine types.
/// </summary>
public sealed class TrainedModel
{
    /// <summary>
    /// Gets or sets the ML context.
    /// </summary>
    public required MLContext MlContext { get; init; }

    /// <summary>
    /// Gets or sets the trained transformer model.
    /// </summary>
    public required ITransformer Transformer { get; init; }

    /// <summary>
    /// Gets or sets the training row CLR type.
    /// </summary>
    public required Type TrainingRowType { get; init; }

    /// <summary>
    /// Gets or sets the prediction CLR type.
    /// </summary>
    public required Type PredictionType { get; init; }
}