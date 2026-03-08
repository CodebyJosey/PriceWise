using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Valora.Application.Abstractions.Listings;
using Valora.Application.Abstractions.PricePrediction;
using Valora.Application.Contracts.Listings;
using Valora.Domain.Constants;
using Valora.Domain.Entities;
using Valora.Infrastructure.Persistence;

namespace Valora.Infrastructure.Services.Listings;

/// <summary>
/// EF Core implementation of the listing service.
/// </summary>
public sealed class ListingService : IListingService
{
    private readonly ValoraDbContext _dbContext;
    private readonly IPricePredictionCategoryRegistry _categoryRegistry;
    private readonly IPricePredictionService _predictionService;
    private readonly IPriceModelVersionStore _versionStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListingService"/> class.
    /// </summary>
    public ListingService(
        ValoraDbContext dbContext,
        IPricePredictionCategoryRegistry categoryRegistry,
        IPricePredictionService predictionService,
        IPriceModelVersionStore versionStore)
    {
        _dbContext = dbContext;
        _categoryRegistry = categoryRegistry;
        _predictionService = predictionService;
        _versionStore = versionStore;
    }

    /// <inheritdoc />
    public async Task<ListingResponse> CreateAsync(
        Guid ownerUserId,
        CreateListingRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateCreateRequest(request);

        EnsureCategoryExists(request.CategoryKey);
        EnsureFeaturesJsonMatchesCategory(request.FeaturesJson, request.CategoryKey);

        Listing listing = new()
        {
            Id = Guid.NewGuid(),
            OwnerUserId = ownerUserId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            CategoryKey = request.CategoryKey.Trim(),
            AskingPrice = request.AskingPrice,
            PredictedPrice = null,
            FeaturesJson = request.FeaturesJson,
            Status = ListingStatuses.Draft,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _dbContext.Listings.Add(listing);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(listing);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ListingResponse>> GetPublishedAsync(
        CancellationToken cancellationToken = default)
    {
        List<Listing> listings = await _dbContext.Listings
            .AsNoTracking()
            .Where(x => x.Status == ListingStatuses.Published)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return listings.Select(Map).ToArray();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ListingResponse>> GetMineAsync(
        Guid ownerUserId,
        CancellationToken cancellationToken = default)
    {
        List<Listing> listings = await _dbContext.Listings
            .AsNoTracking()
            .Where(x => x.OwnerUserId == ownerUserId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return listings.Select(Map).ToArray();
    }

    /// <inheritdoc />
    public async Task<ListingResponse?> GetByIdAsync(
        Guid listingId,
        Guid? requesterUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        Listing? listing = await _dbContext.Listings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == listingId, cancellationToken);

        if (listing is null)
        {
            return null;
        }

        if (!CanAccess(listing, requesterUserId, isAdmin))
        {
            return null;
        }

        return Map(listing);
    }

    /// <inheritdoc />
    public async Task<ListingResponse?> UpdateAsync(
        Guid listingId,
        Guid actorUserId,
        bool isAdmin,
        UpdateListingRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateUpdateRequest(request);

        Listing? listing = await _dbContext.Listings
            .FirstOrDefaultAsync(x => x.Id == listingId, cancellationToken);

        if (listing is null || !CanModify(listing, actorUserId, isAdmin))
        {
            return null;
        }

        EnsureFeaturesJsonMatchesCategory(request.FeaturesJson, listing.CategoryKey);

        listing.Title = request.Title.Trim();
        listing.Description = request.Description.Trim();
        listing.AskingPrice = request.AskingPrice;
        listing.FeaturesJson = request.FeaturesJson;
        listing.UpdatedAtUtc = DateTime.UtcNow;

        if (listing.Status == ListingStatuses.Published)
        {
            listing.Status = listing.PredictedPrice.HasValue
                ? ListingStatuses.Predicted
                : ListingStatuses.Draft;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(listing);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(
        Guid listingId,
        Guid actorUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        Listing? listing = await _dbContext.Listings
            .FirstOrDefaultAsync(x => x.Id == listingId, cancellationToken);

        if (listing is null || !CanModify(listing, actorUserId, isAdmin))
        {
            return false;
        }

        _dbContext.Listings.Remove(listing);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public async Task<ListingResponse?> PredictPriceAsync(
        Guid listingId,
        Guid actorUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        Listing? listing = await _dbContext.Listings
            .FirstOrDefaultAsync(x => x.Id == listingId, cancellationToken);

        if (listing is null || !CanModify(listing, actorUserId, isAdmin))
        {
            return null;
        }

        IPricePredictionCategory category = _categoryRegistry.GetRequired(listing.CategoryKey);

        JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        object? features = JsonSerializer.Deserialize(
            listing.FeaturesJson,
            category.FeaturesType,
            jsonOptions);

        if (features is null)
        {
            throw new InvalidOperationException(
                $"The features payload for listing '{listing.Id}' could not be deserialized to '{category.FeaturesType.Name}'.");
        }

        object prediction = _predictionService.Predict(category.Key, features);
        decimal predictedPrice = ExtractPredictedPrice(prediction);

        int? version = null;
        if (_versionStore.TryGetActive(category.Key, out PriceTrainingMetadata? metadata) && metadata is not null)
        {
            version = metadata.Version;
        }

        listing.PredictedPrice = predictedPrice;
        listing.UpdatedAtUtc = DateTime.UtcNow;

        if (listing.Status == ListingStatuses.Draft)
        {
            listing.Status = ListingStatuses.Predicted;
        }

        PredictionRecord record = new()
        {
            Id = Guid.NewGuid(),
            UserId = actorUserId,
            CategoryKey = listing.CategoryKey,
            FeaturesJson = listing.FeaturesJson,
            PredictedPrice = predictedPrice,
            ModelVersion = version,
            CreatedAtUtc = DateTime.UtcNow
        };

        _dbContext.PredictionRecords.Add(record);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(listing);
    }

    /// <inheritdoc />
    public async Task<ListingResponse?> PublishAsync(
        Guid listingId,
        Guid actorUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        Listing? listing = await _dbContext.Listings
            .FirstOrDefaultAsync(x => x.Id == listingId, cancellationToken);

        if (listing is null || !CanModify(listing, actorUserId, isAdmin))
        {
            return null;
        }

        if (listing.AskingPrice <= 0)
        {
            throw new ArgumentException("A published listing must have an asking price greater than zero.");
        }

        listing.Status = ListingStatuses.Published;
        listing.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(listing);
    }

    /// <inheritdoc />
    public async Task<ListingResponse?> UnpublishAsync(
        Guid listingId,
        Guid actorUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        Listing? listing = await _dbContext.Listings
            .FirstOrDefaultAsync(x => x.Id == listingId, cancellationToken);

        if (listing is null || !CanModify(listing, actorUserId, isAdmin))
        {
            return null;
        }

        listing.Status = listing.PredictedPrice.HasValue
            ? ListingStatuses.Predicted
            : ListingStatuses.Draft;

        listing.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(listing);
    }

    private void ValidateCreateRequest(CreateListingRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Title is required.");
        }

        if (string.IsNullOrWhiteSpace(request.CategoryKey))
        {
            throw new ArgumentException("CategoryKey is required.");
        }

        if (string.IsNullOrWhiteSpace(request.FeaturesJson))
        {
            throw new ArgumentException("FeaturesJson is required.");
        }

        if (request.AskingPrice < 0)
        {
            throw new ArgumentException("AskingPrice cannot be negative.");
        }
    }

    private void ValidateUpdateRequest(UpdateListingRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Title is required.");
        }

        if (string.IsNullOrWhiteSpace(request.FeaturesJson))
        {
            throw new ArgumentException("FeaturesJson is required.");
        }

        if (request.AskingPrice < 0)
        {
            throw new ArgumentException("AskingPrice cannot be negative.");
        }
    }

    private void EnsureCategoryExists(string categoryKey)
    {
        if (!_categoryRegistry.TryGet(categoryKey, out _))
        {
            throw new ArgumentException($"Unknown category '{categoryKey}'.");
        }
    }

    private void EnsureFeaturesJsonMatchesCategory(string featuresJson, string categoryKey)
    {
        IPricePredictionCategory category = _categoryRegistry.GetRequired(categoryKey);

        JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        object? features = JsonSerializer.Deserialize(featuresJson, category.FeaturesType, jsonOptions);

        if (features is null)
        {
            throw new ArgumentException(
                $"The provided FeaturesJson could not be deserialized to '{category.FeaturesType.Name}' for category '{categoryKey}'.");
        }
    }

    private static bool CanModify(Listing listing, Guid actorUserId, bool isAdmin)
        => isAdmin || listing.OwnerUserId == actorUserId;

    private static bool CanAccess(Listing listing, Guid? requesterUserId, bool isAdmin)
        => listing.Status == ListingStatuses.Published
           || isAdmin
           || (requesterUserId.HasValue && listing.OwnerUserId == requesterUserId.Value);

    private static decimal ExtractPredictedPrice(object prediction)
    {
        PropertyInfo? property =
            prediction.GetType().GetProperty("PredictedPrice", BindingFlags.Public | BindingFlags.Instance)
            ?? prediction.GetType().GetProperty("Score", BindingFlags.Public | BindingFlags.Instance);

        if (property is null)
        {
            throw new InvalidOperationException(
                $"Prediction type '{prediction.GetType().Name}' does not expose a PredictedPrice or Score property.");
        }

        object? value = property.GetValue(prediction);

        if (value is null)
        {
            throw new InvalidOperationException("Prediction returned a null predicted value.");
        }

        return value switch
        {
            decimal d => d,
            float f => Convert.ToDecimal(f),
            double db => Convert.ToDecimal(db),
            int i => i,
            long l => l,
            _ when decimal.TryParse(value.ToString(), out decimal parsed) => parsed,
            _ => throw new InvalidOperationException("Prediction value could not be converted to decimal.")
        };
    }

    private static ListingResponse Map(Listing listing)
    {
        return new ListingResponse
        {
            Id = listing.Id,
            OwnerUserId = listing.OwnerUserId,
            Title = listing.Title,
            Description = listing.Description,
            CategoryKey = listing.CategoryKey,
            AskingPrice = listing.AskingPrice,
            PredictedPrice = listing.PredictedPrice,
            Status = listing.Status,
            FeaturesJson = listing.FeaturesJson,
            CreatedAtUtc = listing.CreatedAtUtc,
            UpdatedAtUtc = listing.UpdatedAtUtc
        };
    }
}