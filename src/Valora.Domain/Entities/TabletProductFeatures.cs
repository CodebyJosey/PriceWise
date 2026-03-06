namespace Valora.Domain.Entities;

/// <summary>
/// Represents the features of a tablet used for price prediction.
/// </summary>
public class TabletProductFeatures
{
    /// <summary>
    /// Brand of the tablet.
    /// </summary>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Model family of the tablet.
    /// </summary>
    public string ModelFamily { get; set; } = string.Empty;

    /// <summary>
    /// Storage in GB.
    /// </summary>
    public float StorageGb { get; set; }

    /// <summary>
    /// RAM in GB.
    /// </summary>
    public float RamGb { get; set; }

    /// <summary>
    /// Screen size in inches.
    /// </summary>
    public float ScreenSizeInch { get; set; }

    /// <summary>
    /// Condition of the tablet.
    /// </summary>
    public string Condition { get; set; } = string.Empty;

    /// <summary>
    /// Release year of the tablet.
    /// </summary>
    public float ReleaseYear { get; set; }

    /// <summary>
    /// Indicates whether the tablet supports cellular connectivity.
    /// </summary>
    public string Connectivity { get; set; } = string.Empty;
}