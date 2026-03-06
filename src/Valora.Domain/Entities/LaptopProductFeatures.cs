namespace Valora.Domain.Entities;

/// <summary>
/// Represents the features of a product used for price prediction.
/// </summary>
public class LaptopProductFeatures
{
    /// <summary>
    /// Brand of the product.
    /// </summary>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// CPU model/category.
    /// </summary>
    public string Cpu { get; set; } = string.Empty;

    /// <summary>
    /// RAM in GB.
    /// </summary>
    public float RamGb { get; set; }

    /// <summary>
    /// Storage in GB.
    /// </summary>
    public float StorageGb { get; set; }

    /// <summary>
    /// GPU model/category.
    /// </summary>
    public string Gpu { get; set; } = string.Empty;

    /// <summary>
    /// Screen size in inches.
    /// </summary>
    public float ScreenSizeInch { get; set; }

    /// <summary>
    /// Refresh rate in Hz.
    /// </summary>
    public float RefreshRate { get; set; }

    /// <summary>
    /// Release year of the laptop.
    /// </summary>
    public float ReleaseYear { get; set; }

    /// <summary>
    /// Physical/market condition of the laptop.
    /// </summary>
    public string Condition { get; set; } = string.Empty;

    /// <summary>
    /// Market segment of the laptop.
    /// </summary>
    public string Segment { get; set; } = string.Empty;
}