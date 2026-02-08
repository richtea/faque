using System.Text.Json.Serialization;

namespace HookNorton.Models;

/// <summary>
/// Represents a configured route for the Fake API.
/// </summary>
public class RouteConfiguration
{
    /// <summary>
    /// Gets the HTTP method for this route.
    /// </summary>
    [JsonPropertyName("method")]
    public string Method { get; init; } = string.Empty;

    /// <summary>
    /// Gets the path pattern for this route (e.g., /api/users/{id}).
    /// </summary>
    [JsonPropertyName("pathPattern")]
    public string PathPattern { get; init; } = string.Empty;

    /// <summary>
    /// Gets the configured response for this route.
    /// </summary>
    [JsonPropertyName("response")]
    public RouteResponse Response { get; init; } = new();

    /// <summary>
    /// Gets a value indicating whether this route is enabled.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// Gets or sets the internal version for optimistic concurrency control.
    /// </summary>
    [JsonIgnore]
    public int Version { get; set; }

    /// <summary>
    /// Gets the route key for storage (method + pathPattern).
    /// </summary>
    [JsonIgnore]
    public string Key => $"{Method}:{PathPattern}";
}
