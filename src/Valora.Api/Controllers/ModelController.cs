using Microsoft.AspNetCore.Mvc;
using Valora.Api.Helpers;
using Valora.Application.Abstractions.PricePrediction;

namespace Valora.Api.Controllers;

/// <summary>
/// Generic controller for model training, model info and sanity prediction per category.
/// </summary>
[ApiController]
[Route("api/model")]
public sealed class ModelController : ControllerBase
{
    private readonly IPricePredictionCategoryRegistry _categoryRegistry;
    private readonly IPriceTrainingService _trainingService;
    private readonly IPricePredictionService _predictionService;
    private readonly IPriceModelVersionStore _versionStore;
    private readonly IValoraPathResolver _pathResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelController"/> class.
    /// </summary>
    /// <param name="categoryRegistry">The category registry.</param>
    /// <param name="trainingService">The training service.</param>
    /// <param name="predictionService">The prediction service.</param>
    /// <param name="versionStore">The version store.</param>
    /// <param name="pathResolver">The path resolver.</param>
    public ModelController(
        IPricePredictionCategoryRegistry categoryRegistry,
        IPriceTrainingService trainingService,
        IPricePredictionService predictionService,
        IPriceModelVersionStore versionStore,
        IValoraPathResolver pathResolver)
    {
        _categoryRegistry = categoryRegistry;
        _trainingService = trainingService;
        _predictionService = predictionService;
        _versionStore = versionStore;
        _pathResolver = pathResolver;
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
                modelDirectoryPath = _pathResolver.GetModelDirectoryPath(category),
                metadataDirectoryPath = _pathResolver.GetMetadataDirectoryPath(category),
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
    /// Gets the input feature schema for a category.
    /// </summary>
    /// <param name="category">The category key.</param>
    /// <returns>The feature schema.</returns>
    [HttpGet("{category}/features")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetFeatures([FromRoute] string category)
    {
        if (!_categoryRegistry.TryGet(category, out IPricePredictionCategory? resolvedCategory) || resolvedCategory is null)
        {
            return NotFound(CreateCategoryNotFound(category));
        }

        var features = resolvedCategory.FeaturesType
            .GetProperties()
            .Select(property => new
            {
                name = ToCamelCase(property.Name),
                type = property.PropertyType.Name
            })
            .ToArray();

        return Ok(new
        {
            category = resolvedCategory.Key,
            featuresType = resolvedCategory.FeaturesType.Name,
            featureCount = features.Length,
            features
        });
    }

    /// <summary>
    /// Gets descriptive information for a category and its active model state.
    /// </summary>
    /// <param name="category">The category key.</param>
    /// <returns>The category info.</returns>
    [HttpGet("{category}/info")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetInfo([FromRoute] string category)
    {
        if (!_categoryRegistry.TryGet(category, out IPricePredictionCategory? resolvedCategory) || resolvedCategory is null)
        {
            return NotFound(CreateCategoryNotFound(category));
        }

        string datasetPath = _pathResolver.GetDatasetPath(resolvedCategory);
        string modelDirectoryPath = _pathResolver.GetModelDirectoryPath(resolvedCategory);
        string metadataDirectoryPath = _pathResolver.GetMetadataDirectoryPath(resolvedCategory);

        bool hasActive = _versionStore.TryGetActive(resolvedCategory.Key, out PriceTrainingMetadata? activeMetadata);
        IReadOnlyList<PriceTrainingMetadata> versions = _versionStore.GetAllVersions(resolvedCategory.Key);

        return Ok(new
        {
            category = resolvedCategory.Key,
            datasetFileName = resolvedCategory.DatasetFileName,
            datasetPath,
            modelDirectoryPath,
            metadataDirectoryPath,
            featuresType = resolvedCategory.FeaturesType.Name,
            trainingRowType = resolvedCategory.TrainingRowType.Name,
            predictionType = resolvedCategory.PredictionType.Name,
            datasetExists = System.IO.File.Exists(datasetPath),
            hasActiveVersion = hasActive,
            activeVersion = hasActive ? activeMetadata!.Version : (int?)null,
            activeModelPath = hasActive ? activeMetadata!.ModelPath : null,
            versionCount = versions.Count,
            latestTraining = hasActive ? activeMetadata : null
        });
    }

    /// <summary>
    /// Gets the active model metadata for a category.
    /// </summary>
    /// <param name="category">The category key.</param>
    /// <returns>The active version metadata.</returns>
    [HttpGet("{category}/active")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetActive([FromRoute] string category)
    {
        if (!_categoryRegistry.TryGet(category, out IPricePredictionCategory? resolvedCategory) || resolvedCategory is null)
        {
            return NotFound(CreateCategoryNotFound(category));
        }

        if (!_versionStore.TryGetActive(resolvedCategory.Key, out PriceTrainingMetadata? metadata) || metadata is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Active model not found",
                Detail = $"No active model exists yet for category '{resolvedCategory.Key}'. Train the category first.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(metadata);
    }

    /// <summary>
    /// Gets all known model versions for a category.
    /// </summary>
    /// <param name="category">The category key.</param>
    /// <returns>The version history.</returns>
    [HttpGet("{category}/versions")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetVersions([FromRoute] string category)
    {
        if (!_categoryRegistry.TryGet(category, out IPricePredictionCategory? resolvedCategory) || resolvedCategory is null)
        {
            return NotFound(CreateCategoryNotFound(category));
        }

        IReadOnlyList<PriceTrainingMetadata> versions = _versionStore.GetAllVersions(resolvedCategory.Key);

        return Ok(new
        {
            category = resolvedCategory.Key,
            count = versions.Count,
            versions
        });
    }

    /// <summary>
    /// Gets the latest persisted training metrics for the active version of a category.
    /// </summary>
    /// <param name="category">The category key.</param>
    /// <returns>The latest metrics.</returns>
    [HttpGet("{category}/metrics")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetMetrics([FromRoute] string category)
    {
        if (!_categoryRegistry.TryGet(category, out IPricePredictionCategory? resolvedCategory) || resolvedCategory is null)
        {
            return NotFound(CreateCategoryNotFound(category));
        }

        if (!_versionStore.TryGetActive(resolvedCategory.Key, out PriceTrainingMetadata? metadata) || metadata is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Training metadata not found",
                Detail = $"No active training metadata is available yet for category '{resolvedCategory.Key}'. Train the model first.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(new
        {
            category = metadata.CategoryKey,
            version = metadata.Version,
            trainedAtUtc = metadata.TrainedAtUtc,
            rSquared = metadata.RSquared,
            rootMeanSquaredError = metadata.RootMeanSquaredError,
            meanAbsoluteError = metadata.MeanAbsoluteError,
            meanSquaredError = metadata.MeanSquaredError,
            datasetPath = metadata.DatasetPath,
            modelPath = metadata.ModelPath
        });
    }

    /// <summary>
    /// Trains a model for the specified category.
    /// Creates a new model version and makes it active.
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
                return NotFound(CreateCategoryNotFound(category));
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
    /// Uses the active model version.
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
                return NotFound(CreateCategoryNotFound(category));
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

    private static ProblemDetails CreateCategoryNotFound(string category)
    {
        return new ProblemDetails
        {
            Title = "Category not found",
            Detail = $"No price prediction category is registered with key '{category}'.",
            Status = StatusCodes.Status404NotFound
        };
    }

    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        if (value.Length == 1)
        {
            return value.ToLowerInvariant();
        }

        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}