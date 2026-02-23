using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Faque.Services;

namespace Faque.Startup;

/// <summary>
/// Provides extension methods for configuring application services.
/// </summary>
public static class CompositionRoot
{
    /// <summary>
    /// Configures the application services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAsyncInitializer<DataInitializer>();

        // Add configuration
        services.Configure<FaqueOptions>(
            configuration.GetSection(FaqueOptions.SectionName));

        services.AddProblemDetails();

        // Add services
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false));
            });

        services
            .AddEndpointsApiExplorer()
            .ConfigureApiDocumentation();

        // Add custom services
        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddSingleton<RouteConfigStore>();
        services.AddSingleton<RouteConfigPersistenceService>();
        services.AddSingleton<RequestRecorder>();
        services.AddSingleton<RouteMatcher>();
        services.AddScoped<PersistenceService>();
        services.AddScoped<RequestHistoryCleanupActivity>();

        // Add hosted services
        services.AddHostedService<RouteConfigPersistenceService>();
        services.AddHostedService<RequestHistoryCleanupService>();
    }
}
