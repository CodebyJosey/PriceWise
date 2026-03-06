using Microsoft.ML.Data;

namespace Valora.Infrastructure.ML.Models;

/// <summary>
/// ML.NET prediction output model for laptop prices.
/// </summary>
public sealed class LaptopPricePrediction
{
    [ColumnName("Score")]
    public float PredictedPrice { get; set; }
}