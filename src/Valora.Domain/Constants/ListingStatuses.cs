namespace Valora.Domain.Constants;

/// <summary>
/// Known listing statuses in Valora.
/// </summary>
public static class ListingStatuses
{
    /// <summary>
    /// Listing is created but not yet predicted or published.
    /// </summary>
    public const string Draft = "Draft";

    /// <summary>
    /// Listing has a predicted price.
    /// </summary>
    public const string Predicted = "Predicted";

    /// <summary>
    /// Listing is publicly visible.
    /// </summary>
    public const string Published = "Published";

    /// <summary>
    /// Listing is sold.
    /// </summary>
    public const string Sold = "Sold";

    /// <summary>
    /// Listing is archived.
    /// </summary>
    public const string Archived = "Archived";
}