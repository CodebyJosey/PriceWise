using PriceWise.DatasetGenerator.Abstractions;

namespace PriceWise.DatasetGenerator.Registry;

/// <summary>
/// In-memory registry for dataset category generators.
/// </summary>
public sealed class DatasetCategoryGeneratorRegistry
{
    private readonly Dictionary<string, IDatasetCategoryGenerator> _generators;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatasetCategoryGeneratorRegistry"/> class.
    /// </summary>
    /// <param name="generators">Registered generators.</param>
    public DatasetCategoryGeneratorRegistry(IEnumerable<IDatasetCategoryGenerator> generators)
    {
        _generators = generators.ToDictionary(
            generator => generator.Key,
            generator => generator,
            StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets all registered generators.
    /// </summary>
    /// <returns>All generators.</returns>
    public IReadOnlyCollection<IDatasetCategoryGenerator> GetAll()
        => _generators.Values.ToArray();

    /// <summary>
    /// Gets a generator by key.
    /// </summary>
    /// <param name="key">The category key.</param>
    /// <returns>The generator.</returns>
    public IDatasetCategoryGenerator GetRequired(string key)
    {
        if (!_generators.TryGetValue(key, out IDatasetCategoryGenerator? generator))
        {
            throw new KeyNotFoundException($"No dataset generator is registered with key '{key}'.");
        }

        return generator;
    }

    /// <summary>
    /// Attempts to get a generator by key.
    /// </summary>
    /// <param name="key">The category key.</param>
    /// <param name="generator">The resolved generator.</param>
    /// <returns>True when found; otherwise false.</returns>
    public bool TryGet(string key, out IDatasetCategoryGenerator? generator)
        => _generators.TryGetValue(key, out generator);
}