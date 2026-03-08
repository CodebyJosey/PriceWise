namespace Valora.Application.Contracts.Listings;

/// <summary>
/// Request for updating a listing.
/// </summary>
public sealed class UpdateListingRequest
{
    /// <summary>
    /// Gets or sets the listing title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the listing description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the asking price.
    /// </summary>
    public decimal AskingPrice { get; set; }

    /// <summary>
    /// Gets or sets the serialized category-specific feature payload as JSON.
    /// </summary>
    public string FeaturesJson { get; set; } = string.Empty;
}