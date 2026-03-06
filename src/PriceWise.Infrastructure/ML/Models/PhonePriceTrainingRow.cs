using Microsoft.ML.Data;

namespace PriceWise.Infrastructure.ML.Models;

/// <summary>
/// Represents one row in the phone training dataset.
/// </summary>
public sealed class PhonePriceTrainingRow
{
    [LoadColumn(0)]
    public string Brand { get; set; } = string.Empty;

    [LoadColumn(1)]
    public string ModelFamily { get; set; } = string.Empty;

    [LoadColumn(2)]
    public float StorageGb { get; set; }

    [LoadColumn(3)]
    public float RamGb { get; set; }

    [LoadColumn(4)]
    public float BatteryHealth { get; set; }

    [LoadColumn(5)]
    public string Condition { get; set; } = string.Empty;

    [LoadColumn(6)]
    public float ReleaseYear { get; set; }

    // Label column
    [LoadColumn(7)]
    public float Price { get; set; }
}