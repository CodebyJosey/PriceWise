using PriceWise.Application.Abstractions.PricePrediction;

namespace PriceWise.Infrastructure.ML.Services;

/// <summary>
/// Resolves standard dataset and model paths inside the repository structure.
/// </summary>
public sealed class PriceWisePathResolver : IPriceWisePathResolver
{
    private readonly string _repoRootPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="PriceWisePathResolver"/> class.
    /// </summary>
    /// <param name="repoRootPath">The absolute repository root path.</param>
    public PriceWisePathResolver(string repoRootPath)
    {
        if (string.IsNullOrWhiteSpace(repoRootPath))
        {
            throw new ArgumentException("Repository root path cannot be null or whitespace.", nameof(repoRootPath));
        }

        _repoRootPath = repoRootPath;
    }

    /// <inheritdoc />
    public string GetDatasetPath(IPricePredictionCategory category)
    {
        ArgumentNullException.ThrowIfNull(category);

        return Path.Combine(_repoRootPath, "data", "datasets", category.DatasetFileName);
    }

    /// <inheritdoc />
    public string GetModelPath(IPricePredictionCategory category)
    {
        ArgumentNullException.ThrowIfNull(category);

        return Path.Combine(_repoRootPath, "artifacts", "models", category.ModelFileName);
    }
}