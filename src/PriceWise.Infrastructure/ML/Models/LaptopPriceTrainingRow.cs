using Microsoft.ML.Data;

namespace PriceWise.Infrastructure.ML.Models;

/// <summary>
/// Represents one row in the laptop training dataset.
/// </summary>
public sealed class LaptopPriceTrainingRow
{
    [LoadColumn(0)]
    public string Brand { get; set; } = string.Empty;

    [LoadColumn(1)]
    public string Cpu { get; set; } = string.Empty;

    [LoadColumn(2)]
    public float RamGb { get; set; }

    [LoadColumn(3)]
    public float StorageGb { get; set; }

    [LoadColumn(4)]
    public string Gpu { get; set; } = string.Empty;

    [LoadColumn(5)]
    public float ScreenSizeInch { get; set; }

    [LoadColumn(6)]
    public float RefreshRate { get; set; }

    [LoadColumn(7)]
    public float ReleaseYear { get; set; }

    [LoadColumn(8)]
    public string Condition { get; set; } = string.Empty;

    [LoadColumn(9)]
    public string Segment { get; set; } = string.Empty;

    // Label column
    [LoadColumn(10)]
    public float Price { get; set; }
}