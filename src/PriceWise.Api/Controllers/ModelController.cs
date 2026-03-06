using Microsoft.AspNetCore.Mvc;
using PriceWise.Api.Helpers;
using PriceWise.Application.Abstractions.PricePrediction;

namespace PriceWise.Api.Controllers;

/// <summary>
/// Generic controller for model training and sanity prediction per category.
/// </summary>
[ApiController]
[Route("api/model")]
public sealed class ModelController : ControllerBase
{
    private readonly IPricePredictionCategoryRegistry _categoryRegistry;
    private readonly IPriceTrainingService _trainingService;
    private readonly IPricePredictionService _predictionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelController"/> class.
    /// </summary>
    /// <param name="categoryRegistry">The category registry.</param>
    /// <param name="trainingService">The training service.</param>
    /// <param name="predictionService">The prediction service.</param>
    public ModelController(
        IPricePredictionCategoryRegistry categoryRegistry,
        IPriceTrainingService trainingService,
        IPricePredictionService predictionService)
    {
        _categoryRegistry = categoryRegistry;
        _trainingService = trainingService;
        _predictionService = predictionService;
    }

    /// <summary>
    /// Gets all registered prediction categories.
    /// </summary>
    /// <returns>The available categories.</returns>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetCategories()
    {
        var categories = _categoryRegistry
            .GetAll()
            .OrderBy(category => category.Key, StringComparer.OrdinalIgnoreCase)
            .Select(category => new
            {
                key = category.Key,
                datasetFileName = category.DatasetFileName,
                modelFileName = category.ModelFileName,
                featuresType = category.FeaturesType.Name,
                trainingRowType = category.TrainingRowType.Name,
                predictionType = category.PredictionType.Name
            })
            .ToArray();

        return Ok(new
        {
            count = categories.Length,
            categories
        });
    }

    /// <summary>
    /// Trains a model for the specified category.
    /// </summary>
    /// <param name="category">The category key.</param>
    /// <returns>The training result.</returns>
    [HttpPost("{category}/train")]
    [ProducesResponseType(typeof(PriceTrainingResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult Train([FromRoute] string category)
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

            PriceTrainingResult result = _trainingService.Train(resolvedCategory.Key);
            return Ok(result);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Dataset or model file not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid request",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Training failed",
                Detail = ex.Message,
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Runs a sanity prediction for the specified category using that category's built-in sample input.
    /// </summary>
    /// <param name="category">The category key.</param>
    /// <returns>The sanity prediction result.</returns>
    [HttpPost("{category}/sanity-predict")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult SanityPredict([FromRoute] string category)
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

            object features = resolvedCategory.CreateSanityFeatures();
            object prediction = _predictionService.SanityPredict(resolvedCategory.Key);

            return Ok(PricePredictionResponseFactory.Create(
                resolvedCategory.Key,
                features,
                prediction));
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
                Title = "Sanity prediction failed",
                Detail = ex.Message,
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}