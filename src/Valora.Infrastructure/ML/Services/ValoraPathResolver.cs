using Valora.Application.Abstractions.PricePrediction;

namespace Valora.Infrastructure.ML.Services;

/// <summary>
/// Resolves standard dataset, model and metadata paths inside the repository structure.
/// </summary>
public sealed class ValoraPathResolver : IValoraPathResolver
{
    private readonly string _repoRootPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValoraPathResolver"/> class.
    /// </summary>
    /// <param name="repoRootPath">The absolute repository root path.</param>
    public ValoraPathResolver(string repoRootPath)
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
    public string GetModelDirectoryPath(IPricePredictionCategory category)
    {
        ArgumentNullException.ThrowIfNull(category);

        return Path.Combine(_repoRootPath, "artifacts", "models", category.Key);
    }

    /// <inheritdoc />
    public string GetMetadataDirectoryPath(IPricePredictionCategory category)
    {
        ArgumentNullException.ThrowIfNull(category);

        return Path.Combine(_repoRootPath, "artifacts", "metadata", category.Key);
    }

    /// <inheritdoc />
    public string GetVersionedModelPath(IPricePredictionCategory category, int version)
    {
        ArgumentNullException.ThrowIfNull(category);

        return Path.Combine(GetModelDirectoryPath(category), $"v{version}.zip");
    }

    /// <inheritdoc />
    public string GetVersionedMetadataPath(IPricePredictionCategory category, int version)
    {
        ArgumentNullException.ThrowIfNull(category);

        return Path.Combine(GetMetadataDirectoryPath(category), $"v{version}.json");
    }

    /// <inheritdoc />
    public string GetActiveMetadataPath(IPricePredictionCategory category)
    {
        ArgumentNullException.ThrowIfNull(category);

        return Path.Combine(GetMetadataDirectoryPath(category), "active.json");
    }
}