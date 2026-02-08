using System.IO.Abstractions;
using HookNorton.Middleware;
using HookNorton.Services;
using HookNorton.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

// Load persisted data on startup
var logger = app.Services.GetRequiredService<ILogger<Program>>();
var options = app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<HookNortonOptions>>();
var requestRecorder = app.Services.GetRequiredService<RequestRecorder>();
var fileSystem = app.Services.GetRequiredService<IFileSystem>();
var routePersistence = app.Services.GetRequiredService<RouteConfigPersistenceService>();

// Ensure data directories exist
try
{
    var routeConfigDirectory = fileSystem.Path.GetDirectoryName(options.Value.RouteConfigPath);
    if (!string.IsNullOrEmpty(routeConfigDirectory))
    {
        fileSystem.Directory.CreateDirectory(routeConfigDirectory);
    }

    fileSystem.Directory.CreateDirectory(options.Value.RequestHistoryPath);
    logger.LogInformation("Data directories created/verified");
}
catch (Exception ex)
{
    logger.LogError(ex, "Failed to create data directories");
}

// Load route configuration
routePersistence.LoadRoutesFromDisk();

// Load request history
using (var scope = app.Services.CreateScope())
{
    var persistence = scope.ServiceProvider.GetRequiredService<PersistenceService>();
    var loadResult = persistence.LoadRequests();
    if (loadResult.IsSuccess)
    {
        requestRecorder.LoadRequests(loadResult.Value);
        logger.LogInformation("Loaded {Count} requests from history", loadResult.Value.Count);
    }
    else
    {
        logger.LogWarning("Failed to load request history: {Error}", loadResult.Error.Message);
    }
}

// Configure middleware pipeline

// Converts unhandled exceptions into Problem Details responses
app.UseExceptionHandler();

// Returns the Problem Details response for (empty) non-successful responses
app.UseStatusCodePages();

app.UseStaticFiles(); // Serve static files from wwwroot

// Use Fake API middleware before routing
app.UseFakeApi();

app.UseRouting();

app.MapControllers();

logger.LogInformation("HookNorton starting on HTTP: http://localhost:8080, HTTPS: https://localhost:8081");

app.Run();

/// <summary>
/// The entry point for the application.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
#pragma warning disable ASP0027
public partial class Program;
#pragma warning restore ASP0027
