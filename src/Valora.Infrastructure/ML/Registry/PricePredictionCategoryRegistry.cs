using Valora.Application.Abstractions.PricePrediction;

namespace Valora.Infrastructure.ML.Registry;

/// <summary>
/// Default in-memory registry for all price prediction categories.
/// </summary>
public sealed class PricePredictionCategoryRegistry : IPricePredictionCategoryRegistry
{
    private readonly Dictionary<string, IPricePredictionCategory> _categories;

    /// <summary>
    /// Initializes a new instance of the <see cref="PricePredictionCategoryRegistry"/> class.
    /// </summary>
    /// <param name="categories">All registered categories.</param>
    public PricePredictionCategoryRegistry(IEnumerable<IPricePredictionCategory> categories)
    {
        _categories = categories.ToDictionary(
            category => category.Key,
            category => category,
            StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IPricePredictionCategory> GetAll()
        => _categories.Values.ToArray();

    /// <inheritdoc />
    public IPricePredictionCategory GetRequired(string key)
    {
        if (!_categories.TryGetValue(key, out IPricePredictionCategory? category))
        {
            throw new KeyNotFoundException($"No price prediction category is registered with key '{key}'.");
        }

        return category;
    }

    /// <inheritdoc />
    public bool TryGet(string key, out IPricePredictionCategory? category)
        => _categories.TryGetValue(key, out category);
}