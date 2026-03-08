namespace Valora.Application.Contracts.Listings;

/// <summary>
/// Response for a listing.
/// </summary>
public sealed class ListingResponse
{
    /// <summary>
    /// Gets or sets the listing id.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets or sets the owner user id.
    /// </summary>
    public Guid OwnerUserId { get; init; }

    /// <summary>
    /// Gets or sets the listing title.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Gets or sets the listing description.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Gets or sets the category key.
    /// </summary>
    public required string CategoryKey { get; init; }

    /// <summary>
    /// Gets or sets the asking price.
    /// </summary>
    public decimal AskingPrice { get; init; }

    /// <summary>
    /// Gets or sets the predicted price.
    /// </summary>
    public decimal? PredictedPrice { get; init; }

    /// <summary>
    /// Gets or sets the listing status.
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    /// Gets or sets the serialized features payload as JSON.
    /// </summary>
    public required string FeaturesJson { get; init; }

    /// <summary>
    /// Gets or sets the UTC creation timestamp.
    /// </summary>
    public DateTime CreatedAtUtc { get; init; }

    /// <summary>
    /// Gets or sets the UTC update timestamp.
    /// </summary>
    public DateTime UpdatedAtUtc { get; init; }
}