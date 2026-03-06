using System.Text.Json;
using Valora.Application.Abstractions.PricePrediction;

namespace Valora.Infrastructure.ML.Services;

/// <summary>
/// Stores versioned model metadata as JSON files on disk, one folder per category.
/// </summary>
public sealed class FilePriceModelVersionStore : IPriceModelVersionStore
{
    private readonly IPricePredictionCategoryRegistry _categoryRegistry;
    private readonly IValoraPathResolver _pathResolver;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly object _syncRoot = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="FilePriceModelVersionStore"/> class.
    /// </summary>
    /// <param name="categoryRegistry">The category registry.</param>
    /// <param name="pathResolver">The path resolver.</param>
    public FilePriceModelVersionStore(
        IPricePredictionCategoryRegistry categoryRegistry,
        IValoraPathResolver pathResolver)
    {
        _categoryRegistry = categoryRegistry;
        _pathResolver = pathResolver;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
    }

    /// <inheritdoc />
    public int GetNextVersionNumber(string categoryKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryKey);

        IPricePredictionCategory category = _categoryRegistry.GetRequired(categoryKey);

        lock (_syncRoot)
        {
            string metadataDirectoryPath = _pathResolver.GetMetadataDirectoryPath(category);

            if (!Directory.Exists(metadataDirectoryPath))
            {
                return 1;
            }

            int maxVersion = Directory
                .GetFiles(metadataDirectoryPath, "v*.json", SearchOption.TopDirectoryOnly)
                .Select(TryParseVersionFromFilePath)
                .Where(version => version.HasValue)
                .Select(version => version!.Value)
                .DefaultIfEmpty(0)
                .Max();

            return maxVersion + 1;
        }
    }

    /// <inheritdoc />
    public void SaveVersion(PriceTrainingMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);

        IPricePredictionCategory category = _categoryRegistry.GetRequired(metadata.CategoryKey);

        lock (_syncRoot)
        {
            string metadataDirectoryPath = _pathResolver.GetMetadataDirectoryPath(category);
            Directory.CreateDirectory(metadataDirectoryPath);

            string versionPath = _pathResolver.GetVersionedMetadataPath(category, metadata.Version);
            string activePath = _pathResolver.GetActiveMetadataPath(category);

            string json = JsonSerializer.Serialize(metadata, _jsonOptions);

            File.WriteAllText(versionPath, json);
            File.WriteAllText(activePath, json);
        }
    }

    /// <inheritdoc />
    public bool TryGetActive(string categoryKey, out PriceTrainingMetadata? metadata)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryKey);

        metadata = null;

        if (!_categoryRegistry.TryGet(categoryKey, out IPricePredictionCategory? category) || category is null)
        {
            return false;
        }

        lock (_syncRoot)
        {
            string activePath = _pathResolver.GetActiveMetadataPath(category);

            if (!File.Exists(activePath))
            {
                return false;
            }

            metadata = ReadMetadata(activePath);
            return metadata is not null;
        }
    }

    /// <inheritdoc />
    public bool TryGetVersion(string categoryKey, int version, out PriceTrainingMetadata? metadata)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryKey);

        metadata = null;

        if (!_categoryRegistry.TryGet(categoryKey, out IPricePredictionCategory? category) || category is null)
        {
            return false;
        }

        lock (_syncRoot)
        {
            string versionPath = _pathResolver.GetVersionedMetadataPath(category, version);

            if (!File.Exists(versionPath))
            {
                return false;
            }

            metadata = ReadMetadata(versionPath);
            return metadata is not null;
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<PriceTrainingMetadata> GetAllVersions(string categoryKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryKey);

        IPricePredictionCategory category = _categoryRegistry.GetRequired(categoryKey);

        lock (_syncRoot)
        {
            string metadataDirectoryPath = _pathResolver.GetMetadataDirectoryPath(category);

            if (!Directory.Exists(metadataDirectoryPath))
            {
                return Array.Empty<PriceTrainingMetadata>();
            }

            List<PriceTrainingMetadata> versions = Directory
                .GetFiles(metadataDirectoryPath, "v*.json", SearchOption.TopDirectoryOnly)
                .Select(ReadMetadata)
                .Where(metadata => metadata is not null)
                .Select(metadata => metadata!)
                .OrderByDescending(metadata => metadata.Version)
                .ToList();

            return versions;
        }
    }

    /// <inheritdoc />
    public bool SetActive(string categoryKey, int version)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryKey);

        if (!_categoryRegistry.TryGet(categoryKey, out IPricePredictionCategory? category) || category is null)
        {
            return false;
        }

        lock (_syncRoot)
        {
            string versionPath = _pathResolver.GetVersionedMetadataPath(category, version);
            string activePath = _pathResolver.GetActiveMetadataPath(category);

            if (!File.Exists(versionPath))
            {
                return false;
            }

            string json = File.ReadAllText(versionPath);
            File.WriteAllText(activePath, json);

            return true;
        }
    }

    private PriceTrainingMetadata? ReadMetadata(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<PriceTrainingMetadata>(json, _jsonOptions);
    }

    private static int? TryParseVersionFromFilePath(string filePath)
    {
        string fileName = Path.GetFileNameWithoutExtension(filePath);

        if (!fileName.StartsWith('v'))
        {
            return null;
        }

        return int.TryParse(fileName[1..], out int version)
            ? version
            : null;
    }
}