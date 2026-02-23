namespace Faque.Models;

/// <summary>
/// Data model for creating/updating routes.
/// </summary>
[PublicAPI]
public class RouteUpdateRequestModel
{
    /// <summary>
    /// Gets or sets the response configuration for the route.
    /// </summary>
    public required RouteResponse Response { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the route is enabled.
    /// </summary>
    public bool Enabled { get; init; } = true;
}
