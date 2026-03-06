using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Valora.Api.Helpers;
using Valora.Application.Abstractions.PricePrediction;

namespace Valora.Api.Controllers;

/// <summary>
/// Generic controller for price predictions per category.
/// </summary>
[ApiController]
[Route("api/predictions")]
public sealed class PredictionController : ControllerBase
{
    private readonly IPricePredictionCategoryRegistry _categoryRegistry;
    private readonly IPricePredictionService _predictionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PredictionController"/> class.
    /// </summary>
    /// <param name="categoryRegistry">The category registry.</param>
    /// <param name="predictionService">The prediction service.</param>
    public PredictionController(
        IPricePredictionCategoryRegistry categoryRegistry,
        IPricePredictionService predictionService)
    {
        _categoryRegistry = categoryRegistry;
        _predictionService = predictionService;
    }

    /// <summary>
    /// Predicts a price for the given category using the posted category-specific features payload.
    /// </summary>
    /// <param name="category">The category key.</param>
    /// <param name="body">The category-specific JSON body.</param>
    /// <returns>The prediction result.</returns>
    [HttpPost("{category}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult Predict(
        [FromRoute] string category,
        [FromBody] JsonElement body)
    {
        try
        {
            if (!_categoryRegistry.TryGet(category, out IPricePredictionCategory? resolvedCategory) || resolvedCategory is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Category not found",
                    Detail = $"No price prediction category is registered with key '{category}'.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            object features = CategoryFeatureDeserializer.Deserialize(body, resolvedCategory);
            object prediction = _predictionService.Predict(resolvedCategory.Key, features);

            return Ok(PricePredictionResponseFactory.Create(
                resolvedCategory.Key,
                features,
                prediction));
        }
        catch (JsonException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid JSON body",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid prediction input",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Model file not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Prediction failed",
                Detail = ex.Message,
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}