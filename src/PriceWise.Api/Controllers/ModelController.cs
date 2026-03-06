using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using PriceWise.Infrastructure.ML.Models;
using PriceWise.Infrastructure.ML.Training;
using PriceWise.Infrastructure.ML.Prediction;
using PriceWise.Infrastructure.ML.Definitions;

namespace PriceWise.Api.Controllers;

[ApiController]
[Route("api/model")]
public sealed class ModelController : ControllerBase
{
    private readonly IHostEnvironment _env;
    private readonly ITrainedModelProvider _modelProvider;

    public ModelController(IHostEnvironment env, ITrainedModelProvider modelProvider)
    {
        _env = env;
        _modelProvider = modelProvider;
    }

    /// <summary>
    /// Trains a new ML model and saves it to artifacts/models/laptop-price-model.zip.
    /// Reloads the model in-memory afterwards so prediction uses the latest model without restarting.
    /// </summary>
    [HttpPost("train/laptops")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult Train()
    {
        string repoRoot = Directory.GetParent(_env.ContentRootPath)!.Parent!.FullName;

        string dataSetPath = Path.Combine(repoRoot, "data", "datasets", "laptops.csv");
        string modelPath = Path.Combine(repoRoot, "artifacts", "models", "laptop-price-model.zip");

        try
        {
            MLContext ml = new MLContext(seed: 1);
            LaptopPriceModelDefinition? definition = new LaptopPriceModelDefinition();
            TabularRegressionTrainer<LaptopPriceTrainingRow, LaptopPricePrediction>? trainer = new TabularRegressionTrainer<LaptopPriceTrainingRow, LaptopPricePrediction>(ml, definition);
            RegressionTrainingResult? result = trainer.TrainEvaluateAndSave(dataSetPath, modelPath);

            // Ensure the API starts using the freshly trained model immediately.
            _modelProvider.Reload();

            FileInfo? info = new FileInfo(modelPath);

            return Ok(new
            {
                message = "Model trained, saved and reloaded.",
                dataSetPath,
                modelPath,
                rowCount = result.RowCount,
                rmse = result.Rmse,
                rSquared = result.RSquared,
                sanityPrediction = result.SanityPrediction, // if you included it
                sizeBytes = info.Length,
                lastWriteUtc = info.LastWriteTimeUtc
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                Problem(title: "Training failed", detail: ex.Message));
        }
    }

    /// <summary>
    /// Returns basic info about the current model file (exists, size, last write).
    /// </summary>
    [HttpGet("info")]
    public IActionResult Info()
    {
        string repoRoot = Directory.GetParent(_env.ContentRootPath)!.Parent!.FullName;
        string modelPath = Path.Combine(repoRoot, "artifacts", "models", "laptop-price-model.zip");

        FileInfo? file = new FileInfo(modelPath);

        return Ok(new
        {
            modelPath,
            exists = file.Exists,
            sizeBytes = file.Exists ? file.Length : 0,
            lastWriteUtc = file.Exists ? file.LastWriteTimeUtc : (DateTime?)null
        });
    }

    /// <summary>
    /// Quick check: load first few rows from the dataset as ML.NET sees them.
    /// Helps debug CSV delimiter/column mapping issues.
    /// </summary>
    [HttpGet("peek")]
    public IActionResult Peek()
    {
        string repoRoot = Directory.GetParent(_env.ContentRootPath)!.Parent!.FullName;
        string csvPath = Path.Combine(repoRoot, "data", "datasets", "laptops.csv");

        MLContext? ml = new MLContext(seed: 1);

        // Keep separatorChar consistent with your dataset
        IDataView data = ml.Data.LoadFromTextFile<LaptopPriceTrainingRow>(
            path: csvPath,
            hasHeader: true,
            separatorChar: ',');

        List<LaptopPriceTrainingRow>? sample = ml.Data.CreateEnumerable<LaptopPriceTrainingRow>(data, reuseRowObject: false)
            .Take(5)
            .ToList();

        return Ok(new
        {
            csvPath,
            sample
        });
    }

    /// <summary>
    /// Sanity prediction endpoint that uses the currently loaded model (via ITrainedModelProvider)
    /// and reads the Score directly from an IDataView (very reliable).
    /// </summary>
    [HttpPost("sanity-predict")]
    public IActionResult SanityPredict([FromBody] PredictPriceRequest request)
    {
        if (request is null)
        {
            return BadRequest(Problem("Request body is required."));
        }

        ITransformer model = _modelProvider.GetModel();

        MLContext? ml = new MLContext(seed: 1);

        LaptopPriceTrainingRow? input = new LaptopPriceTrainingRow
        {
            Brand = request.Brand.Trim(),
            Cpu = request.Cpu.Trim(),
            RamGb = request.RamGb,
            StorageGb = request.StorageGb,
            Gpu = request.Gpu.Trim().Replace(" ", ""),
            Price = 0
        };

        IDataView inputView = ml.Data.LoadFromEnumerable(new[] { input });
        IDataView scored = model.Transform(inputView);

        float score = ml.Data.CreateEnumerable<ScoreRow>(scored, reuseRowObject: false)
            .First()
            .Score;

        return Ok(new
        {
            input,
            predictedPrice = score
        });
    }

    private sealed class ScoreRow
    {
        public float Score { get; set; }
    }

    public sealed record PredictPriceRequest(
        string Brand,
        string Cpu,
        float RamGb,
        float StorageGb,
        string Gpu);
}
