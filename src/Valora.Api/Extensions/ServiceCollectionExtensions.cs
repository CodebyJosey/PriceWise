using Valora.Application.Abstractions.PricePrediction;
using Valora.Infrastructure.ML.Definitions;
using Valora.Infrastructure.ML.Registry;
using Valora.Infrastructure.ML.Services;

namespace Valora.Api.Extensions;

/// <summary>
/// Extension methods for registering Valora services.
/// Keeps Program.cs clean and organizes DI by layer.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Valora infrastructure and application services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="contentRootPath">The ASP.NET content root path.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddValoraServices(
        this IServiceCollection services,
        string contentRootPath)
    {
        string repoRootPath = Directory.GetParent(contentRootPath)!.Parent!.FullName;

        services.AddSingleton<IValoraPathResolver>(_ => new ValoraPathResolver(repoRootPath));

        services.AddSingleton<IPricePredictionCategory, LaptopPriceModelDefinition>();
        services.AddSingleton<IPricePredictionCategory, PhonePriceModelDefinition>();
        services.AddSingleton<IPricePredictionCategory, TabletPriceModelDefinition>();

        services.AddSingleton<IPricePredictionCategoryRegistry, PricePredictionCategoryRegistry>();

        services.AddSingleton<IPriceModelVersionStore, FilePriceModelVersionStore>();
        services.AddSingleton<ITrainedModelCatalog, FileTrainedModelCatalog>();

        services.AddSingleton<IPricePredictionService, MlPricePredictionService>();
        services.AddSingleton<IPriceTrainingService, MlPriceTrainingService>();

        return services;
    }
}