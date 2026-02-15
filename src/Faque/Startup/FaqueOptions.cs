namespace Faque.Startup;

/// <summary>
/// Configuration options for Faque.
/// </summary>
public class FaqueOptions
{
    public const string SectionName = "Faque";

    /// <summary>
    /// Gets or sets the maximum number of requests to keep in history.
    /// </summary>
    public int MaxRequestHistory { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the maximum request body size in bytes.
    /// </summary>
    public int MaxBodySize { get; set; } = 1_024 * 50; // 50 KB

    /// <summary>
    /// Gets or sets the interval in seconds for cleanup of stale request history files.
    /// </summary>
    public int CleanupIntervalSeconds { get; set; } = 60;

    /// <summary>
    /// Gets or sets the debounce period in seconds for persisting route configuration changes.
    /// </summary>
    public int RouteConfigDebounceSeconds { get; set; } = 2;

    /// <summary>
    /// Gets or sets the directory path for persisted data.
    /// </summary>
    public string DataDirectory { get; set; } = "./data";

    /// <summary>
    /// Gets the path to the route configuration file.
    /// </summary>
    public string RouteConfigPath => Path.Combine(DataDirectory, "config", "routes.json");

    /// <summary>
    /// Gets the path to the request history directory.
    /// </summary>
    public string RequestHistoryPath => Path.Combine(DataDirectory, "history");
}
