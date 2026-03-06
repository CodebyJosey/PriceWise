namespace Valora.DatasetGenerator.Rows;

/// <summary>
/// CSV row for phone dataset generation.
/// </summary>
public sealed class PhoneDatasetRow
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
    /// Gets or sets storage in GB.
    /// </summary>
    public float StorageGb { get; set; }

    /// <summary>
    /// Gets or sets RAM in GB.
    /// </summary>
    public float RamGb { get; set; }

    /// <summary>
    /// Gets or sets battery health percentage.
    /// </summary>
    public float BatteryHealth { get; set; }

    /// <summary>
    /// Gets or sets condition.
    /// </summary>
    public string Condition { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets release year.
    /// </summary>
    public float ReleaseYear { get; set; }

    /// <summary>
    /// Gets or sets price.
    /// </summary>
    public float Price { get; set; }
}