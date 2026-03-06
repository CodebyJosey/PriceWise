using Microsoft.ML.Data;

namespace Valora.Infrastructure.ML.Models;

/// <summary>
/// ML.NET input row for tablet price training and prediction.
/// Property names match the tablet dataset columns.
/// </summary>
public sealed class TabletPriceTrainingRow
{
    /// <summary>
    /// Tablet brand.
    /// </summary>
    [LoadColumn(0)]
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Tablet model family.
    /// </summary>
    [LoadColumn(1)]
    public string ModelFamily { get; set; } = string.Empty;

    /// <summary>
    /// Storage in GB.
    /// </summary>
    [LoadColumn(2)]
    public float StorageGb { get; set; }

    /// <summary>
    /// RAM in GB.
    /// </summary>
    [LoadColumn(3)]
    public float RamGb { get; set; }

    /// <summary>
    /// Screen size in inches.
    /// </summary>
    [LoadColumn(4)]
    public float ScreenSizeInch { get; set; }

    /// <summary>
    /// Condition of the tablet.
    /// </summary>
    [LoadColumn(5)]
    public string Condition { get; set; } = string.Empty;

    /// <summary>
    /// Release year.
    /// </summary>
    [LoadColumn(6)]
    public float ReleaseYear { get; set; }

    /// <summary>
    /// Connectivity type.
    /// </summary>
    [LoadColumn(7)]
    public string Connectivity { get; set; } = string.Empty;

    /// <summary>
    /// Price label.
    /// </summary>
    [LoadColumn(8)]
    public float Price { get; set; }
}