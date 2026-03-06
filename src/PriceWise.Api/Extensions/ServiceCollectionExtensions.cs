using PriceWise.Application.Abstractions.PricePrediction;
using PriceWise.Infrastructure.ML.Definitions;
using PriceWise.Infrastructure.ML.Registry;
using PriceWise.Infrastructure.ML.Services;

namespace PriceWise.Api.Extensions;

/// <summary>
/// Extension methods for registering PriceWise services.
/// Keeps Program.cs clean and organizes DI by layer.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers PriceWise infrastructure and application services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="contentRootPath">The ASP.NET content root path.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddPriceWiseServices(
        this IServiceCollection services,
        string contentRootPath)
    {
        string repoRootPath = Directory.GetParent(contentRootPath)!.Parent!.FullName;

        services.AddSingleton<IPriceWisePathResolver>(_ => new PriceWisePathResolver(repoRootPath));

        services.AddSingleton<IPricePredictionCategory, LaptopPriceModelDefinition>();
        services.AddSingleton<IPricePredictionCategory, PhonePriceModelDefinition>();
        services.AddSingleton<IPricePredictionCategory, TabletPriceModelDefinition>();

        services.AddSingleton<IPricePredictionCategoryRegistry, PricePredictionCategoryRegistry>();

        services.AddSingleton<ITrainedModelCatalog, FileTrainedModelCatalog>();
        services.AddSingleton<IPricePredictionService, MlPricePredictionService>();
        services.AddSingleton<IPriceTrainingService, MlPriceTrainingService>();

        return services;
    }
}