using Valora.Application.Contracts.Listings;

namespace Valora.Application.Abstractions.Listings;

/// <summary>
/// Service for managing marketplace listings.
/// </summary>
public interface IListingService
{
    /// <summary>
    /// Creates a new listing.
    /// </summary>
    Task<ListingResponse> CreateAsync(
        Guid ownerUserId,
        CreateListingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all published listings.
    /// </summary>
    Task<IReadOnlyCollection<ListingResponse>> GetPublishedAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all listings owned by the given user.
    /// </summary>
    Task<IReadOnlyCollection<ListingResponse>> GetMineAsync(
        Guid ownerUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single listing if accessible to the requester.
    /// </summary>
    Task<ListingResponse?> GetByIdAsync(
        Guid listingId,
        Guid? requesterUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing listing.
    /// </summary>
    Task<ListingResponse?> UpdateAsync(
        Guid listingId,
        Guid actorUserId,
        bool isAdmin,
        UpdateListingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a listing.
    /// </summary>
    Task<bool> DeleteAsync(
        Guid listingId,
        Guid actorUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Predicts and stores the price for a listing.
    /// </summary>
    Task<ListingResponse?> PredictPriceAsync(
        Guid listingId,
        Guid actorUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a listing.
    /// </summary>
    Task<ListingResponse?> PublishAsync(
        Guid listingId,
        Guid actorUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unpublishes a listing.
    /// </summary>
    Task<ListingResponse?> UnpublishAsync(
        Guid listingId,
        Guid actorUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default);
}