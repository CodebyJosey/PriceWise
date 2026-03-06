namespace PriceWise.DatasetGenerator.Rows;

/// <summary>
/// CSV row for laptop dataset generation.
/// </summary>
public sealed class LaptopDatasetRow
{
    /// <summary>
    /// Gets or sets the brand.
    /// </summary>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the CPU.
    /// </summary>
    public string Cpu { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets RAM in GB.
    /// </summary>
    public float RamGb { get; set; }

    /// <summary>
    /// Gets or sets storage in GB.
    /// </summary>
    public float StorageGb { get; set; }

    /// <summary>
    /// Gets or sets the GPU.
    /// </summary>
    public string Gpu { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets screen size in inches.
    /// </summary>
    public float ScreenSizeInch { get; set; }

    /// <summary>
    /// Gets or sets refresh rate.
    /// </summary>
    public float RefreshRate { get; set; }

    /// <summary>
    /// Gets or sets release year.
    /// </summary>
    public float ReleaseYear { get; set; }

    /// <summary>
    /// Gets or sets condition.
    /// </summary>
    public string Condition { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets market segment.
    /// </summary>
    public string Segment { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets price.
    /// </summary>
    public float Price { get; set; }
}