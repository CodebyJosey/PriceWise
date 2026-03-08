using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Valora.Application.Abstractions.Listings;
using Valora.Application.Contracts.Listings;
using Valora.Infrastructure.Persistence.Identity;

namespace Valora.Api.Controllers;

/// <summary>
/// Listing endpoints for the Valora marketplace.
/// </summary>
[ApiController]
[Route("api/listings")]
public sealed class ListingsController : ControllerBase
{
    private readonly IListingService _listingService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListingsController"/> class.
    /// </summary>
    public ListingsController(IListingService listingService)
    {
        _listingService = listingService;
    }

    /// <summary>
    /// Gets all published listings.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ListingResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublished(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<ListingResponse> listings =
            await _listingService.GetPublishedAsync(cancellationToken);

        return Ok(listings);
    }

    /// <summary>
    /// Gets the current user's own listings.
    /// </summary>
    [Authorize]
    [HttpGet("mine")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ListingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        Guid userId = GetRequiredUserId();

        IReadOnlyCollection<ListingResponse> listings =
            await _listingService.GetMineAsync(userId, cancellationToken);

        return Ok(listings);
    }

    /// <summary>
    /// Gets a single listing if accessible to the current requester.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ListingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        Guid? userId = TryGetUserId();
        bool isAdmin = User.IsInRole(ApplicationRoles.Admin);

        ListingResponse? listing =
            await _listingService.GetByIdAsync(id, userId, isAdmin, cancellationToken);

        if (listing is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Listing not found",
                Detail = $"No accessible listing was found with id '{id}'.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(listing);
    }

    /// <summary>
    /// Creates a new listing.
    /// </summary>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ListingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(
        [FromBody] CreateListingRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = GetRequiredUserId();

            ListingResponse response =
                await _listingService.CreateAsync(userId, request, cancellationToken);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid listing request",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    /// <summary>
    /// Updates an existing listing.
    /// </summary>
    [Authorize]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ListingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateListingRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = GetRequiredUserId();
            bool isAdmin = User.IsInRole(ApplicationRoles.Admin);

            ListingResponse? response =
                await _listingService.UpdateAsync(id, userId, isAdmin, request, cancellationToken);

            if (response is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Listing not found",
                    Detail = $"No editable listing was found with id '{id}'.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid update request",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    /// <summary>
    /// Deletes an existing listing.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        Guid userId = GetRequiredUserId();
        bool isAdmin = User.IsInRole(ApplicationRoles.Admin);

        bool deleted = await _listingService.DeleteAsync(id, userId, isAdmin, cancellationToken);

        if (!deleted)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Listing not found",
                Detail = $"No deletable listing was found with id '{id}'.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Predicts and stores the price of a listing.
    /// </summary>
    [Authorize]
    [HttpPost("{id:guid}/predict-price")]
    [ProducesResponseType(typeof(ListingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PredictPrice([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = GetRequiredUserId();
            bool isAdmin = User.IsInRole(ApplicationRoles.Admin);

            ListingResponse? response =
                await _listingService.PredictPriceAsync(id, userId, isAdmin, cancellationToken);

            if (response is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Listing not found",
                    Detail = $"No editable listing was found with id '{id}'.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Prediction failed",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Prediction failed",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Model not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
    }

    /// <summary>
    /// Publishes a listing.
    /// </summary>
    [Authorize]
    [HttpPost("{id:guid}/publish")]
    [ProducesResponseType(typeof(ListingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Publish([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = GetRequiredUserId();
            bool isAdmin = User.IsInRole(ApplicationRoles.Admin);

            ListingResponse? response =
                await _listingService.PublishAsync(id, userId, isAdmin, cancellationToken);

            if (response is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Listing not found",
                    Detail = $"No publishable listing was found with id '{id}'.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Publish failed",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    /// <summary>
    /// Unpublishes a listing.
    /// </summary>
    [Authorize]
    [HttpPost("{id:guid}/unpublish")]
    [ProducesResponseType(typeof(ListingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unpublish([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        Guid userId = GetRequiredUserId();
        bool isAdmin = User.IsInRole(ApplicationRoles.Admin);

        ListingResponse? response =
            await _listingService.UnpublishAsync(id, userId, isAdmin, cancellationToken);

        if (response is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Listing not found",
                Detail = $"No unpublishable listing was found with id '{id}'.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(response);
    }

    private Guid GetRequiredUserId()
    {
        Guid? userId = TryGetUserId();

        if (!userId.HasValue)
        {
            throw new InvalidOperationException("The current user id could not be determined from the JWT.");
        }

        return userId.Value;
    }

    private Guid? TryGetUserId()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out Guid parsed))
        {
            return null;
        }

        return parsed;
    }
}