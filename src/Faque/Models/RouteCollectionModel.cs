namespace Faque.Models;

public class RouteCollectionModel
{
    /// <summary>
    /// Gets the collection of route configurations used to define API routes.
    /// Each route specifies its HTTP method, path pattern, response configuration, and status.
    /// </summary>
    public required IEnumerable<RouteConfiguration> Routes { get; init; }
}
