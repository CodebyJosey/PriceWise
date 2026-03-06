namespace Valora.DatasetGenerator.Rows;

/// <summary>
/// CSV row for tablet dataset generation.
/// </summary>
public sealed class TabletDatasetRow
{
    /// <summary>
    /// Gets or sets the brand.
    /// </summary>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the model family.
    /// </summary>
    public string ModelFamily { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the storage in GB.
    /// </summary>
    public float StorageGb { get; set; }

    /// <summary>
    /// Gets or sets the RAM in GB.
    /// </summary>
    public float RamGb { get; set; }

    /// <summary>
    /// Gets or sets the screen size in inches.
    /// </summary>
    public float ScreenSizeInch { get; set; }

    /// <summary>
    /// Gets or sets the condition.
    /// </summary>
    public string Condition { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the release year.
    /// </summary>
    public float ReleaseYear { get; set; }

    /// <summary>
    /// Gets or sets the connectivity type.
    /// </summary>
    public string Connectivity { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the price.
    /// </summary>
    public float Price { get; set; }
}