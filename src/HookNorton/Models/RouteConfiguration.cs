using System.Text.Json.Serialization;

namespace HookNorton.Models;

/// <summary>
/// Represents a configured route for the Fake API.
/// </summary>
[PublicAPI]
public class RouteConfiguration
{
    /// <summary>
    /// Gets the HTTP method for this route.
    /// </summary>
    public string Method { get; init; } = string.Empty;

    /// <summary>
    /// Gets the path pattern for this route (e.g., /api/users/{id}).
    /// </summary>
    public string PathPattern { get; init; } = string.Empty;

    /// <summary>
    /// Gets the configured response for this route.
    /// </summary>
    public RouteResponse Response { get; init; } = new();

    /// <summary>
    /// Gets a value indicating whether this route is enabled.
    /// </summary>
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
