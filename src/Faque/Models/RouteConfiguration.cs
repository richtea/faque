using System.Text.Json.Serialization;

namespace Faque.Models;

/// <summary>
/// Represents a configured route for the Fake API.
/// </summary>
[PublicAPI]
public class RouteConfiguration
{
    /// <summary>
    /// Gets the route key for storage (method + path pattern).
    /// </summary>
    [JsonIgnore]
    public string Key => $"{Method}:{PathPattern}";

    /// <summary>
    /// Gets the HTTP method for this route.
    /// </summary>
    /// <example>GET</example>
    public required string Method { get; init; }

    /// <summary>
    /// Gets the path pattern for this route (e.g., /api/users/*).
    /// </summary>
    public required string PathPattern { get; init; }

    /// <summary>
    /// Gets the configured response for this route.
    /// </summary>
    public required RouteResponse Response { get; init; }

    /// <summary>
    /// Gets a value indicating whether this route is enabled.
    /// </summary>
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// Gets or sets the internal version for optimistic concurrency control.
    /// </summary>
    [JsonIgnore]
    public int Version { get; set; }
}
