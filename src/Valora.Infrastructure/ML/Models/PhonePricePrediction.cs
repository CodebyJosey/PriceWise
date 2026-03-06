using Microsoft.ML.Data;

namespace Valora.Infrastructure.ML.Models;

/// <summary>
/// ML.NET prediction output model for phone prices.
/// </summary>
public sealed class PhonePricePrediction
{
    [ColumnName("Score")]
    public float PredictedPrice { get; set; }
}