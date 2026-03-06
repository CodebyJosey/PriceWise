namespace Valora.Domain.Entities;

/// <summary>
/// Represents the features of a product used for price prediction.
/// </summary>
public class PhoneProductFeatures
{
    /// <summary>
    /// Brand of the product.
    /// </summary>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Model of the brand.
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
    /// Battery health in percentage.
    /// </summary>
    public float BatteryHealth { get; set; }

    /// <summary>
    /// Physical/market condition of the laptop.
    /// </summary>
    public string Condition { get; set; } = string.Empty;
    
    /// <summary>
    /// Release year of the laptop.
    /// </summary>
    public float ReleaseYear { get; set; }
}