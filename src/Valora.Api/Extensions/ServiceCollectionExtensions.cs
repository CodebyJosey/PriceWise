using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Valora.Application.Abstractions.PricePrediction;
using Valora.Infrastructure.ML.Definitions;
using Valora.Infrastructure.ML.Registry;
using Valora.Infrastructure.ML.Services;
using Valora.Infrastructure.Persistence;
using Valora.Infrastructure.Persistence.Identity;

namespace Valora.Api.Extensions;

/// <summary>
/// Extension methods for registering Valora services.
/// Keeps Program.cs clean and organizes DI by layer.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers database and identity services for Valora.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddValoraPersistence(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionString 'DefaultConnection' was not found."
            );
        }

        services.AddDbContext<ValoraDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<ValoraDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        return services;
    }

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