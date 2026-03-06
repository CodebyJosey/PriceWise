using System.Text.Json;
using Valora.Application.Abstractions.PricePrediction;

namespace Valora.Api.Helpers;

/// <summary>
/// Deserializes incoming JSON into the correct feature type for a category.
/// </summary>
public static class CategoryFeatureDeserializer
{
    /// <summary>
    /// Deserializes the request body into the category-specific features type.
    /// </summary>
    /// <param name="json">The raw JSON body.</param>
    /// <param name="category">The resolved category.</param>
    /// <returns>The deserialized features object.</returns>
    public static object Deserialize(JsonElement json, IPricePredictionCategory category)
    {
        ArgumentNullException.ThrowIfNull(category);

        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        string rawJson = json.GetRawText();

        object? result = JsonSerializer.Deserialize(rawJson, category.FeaturesType, options);

        if (result is null)
        {
            throw new InvalidOperationException(
                $"The request body could not be deserialized to '{category.FeaturesType.Name}' for category '{category.Key}'.");
        }

        return result;
    }
}