namespace Valora.Domain.Entities;

/// <summary>
/// Represents a marketplace listing.
/// </summary>
public class Listing
{
    /// <summary>
    /// Gets or sets the unique identifier of the listing.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the owner user id.
    /// </summary>
    public Guid OwnerUserId { get; set; }

    /// <summary>
    /// Gets or sets the listing title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the listing description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category key.
    /// Example: laptops, phones, tablets.
    /// </summary>
    public string CategoryKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the requested asking price.
    /// </summary>
    public decimal AskingPrice { get; set; }

    /// <summary>
    /// Gets or sets the latest predicted price.
    /// </summary>
    public decimal? PredictedPrice { get; set; }

    /// <summary>
    /// Gets or sets the serialized feature payload in JSON format.
    /// </summary>
    public string FeaturesJson { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current listing status.
    /// </summary>
    public string Status { get; set; } = "Draft";

    /// <summary>
    /// Gets or sets the UTC creation timestamp.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the UTC update timestamp.
    /// </summary>
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}