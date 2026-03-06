using System.Reflection;

namespace PriceWise.Api.Helpers;

/// <summary>
/// Creates consistent API responses for price predictions.
/// </summary>
public static class PricePredictionResponseFactory
{
    /// <summary>
    /// Creates a response object for a prediction result.
    /// </summary>
    /// <param name="categoryKey">The category key.</param>
    /// <param name="features">The original features object.</param>
    /// <param name="prediction">The raw prediction object returned by ML.NET.</param>
    /// <returns>A serializable response object.</returns>
    public static object Create(string categoryKey, object? features, object prediction)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryKey);
        ArgumentNullException.ThrowIfNull(prediction);

        float? predictedPrice = TryExtractPredictedPrice(prediction);

        return new
        {
            category = categoryKey,
            predictedPrice,
            features,
            rawPrediction = prediction
        };
    }

    /// <summary>
    /// Attempts to extract a predicted price value from the prediction object.
    /// </summary>
    /// <param name="prediction">The prediction object.</param>
    /// <returns>The predicted price when found; otherwise null.</returns>
    private static float? TryExtractPredictedPrice(object prediction)
    {
        Type type = prediction.GetType();

        PropertyInfo? property =
            type.GetProperty("PredictedPrice", BindingFlags.Public | BindingFlags.Instance)
            ?? type.GetProperty("Score", BindingFlags.Public | BindingFlags.Instance);

        if (property is null)
        {
            return null;
        }

        object? value = property.GetValue(prediction);

        if (value is null)
        {
            return null;
        }

        if (value is float f)
        {
            return f;
        }

        if (value is double d)
        {
            return (float)d;
        }

        if (float.TryParse(value.ToString(), out float parsed))
        {
            return parsed;
        }

        return null;
    }
}