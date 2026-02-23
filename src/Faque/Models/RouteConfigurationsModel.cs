namespace Faque.Models;

public class RouteConfigurationsModel
{
    /// <summary>
    /// Gets the collection of route configurations used to define API routes.
    /// Each route specifies its HTTP method and path pattern to match against, and the response configuration,
    /// including the HTTP status, response body, and headers to return.
    /// </summary>
    public required IEnumerable<RouteConfiguration> Routes { get; init; }
}
