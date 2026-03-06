using Microsoft.ML.Data;

namespace PriceWise.Infrastructure.ML.Models;

/// <summary>
/// ML.NET prediction output for tablet price prediction.
/// </summary>
public sealed class TabletPricePrediction
{
    /// <summary>
    /// Predicted price.
    /// </summary>
    [ColumnName("Score")]
    public float PredictedPrice { get; set; }
}