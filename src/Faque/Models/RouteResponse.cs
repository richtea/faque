using Faque.Common.AspNet;

namespace Faque.Models;

/// <summary>
/// Represents the response configuration for a route.
/// </summary>
[PublicAPI]
public class RouteResponse
{
    /// <summary>
    /// Gets or sets the HTTP status code to return.
    /// </summary>
    public int StatusCode { get; set; } = 200;

    /// <summary>
    /// Gets or sets the HTTP headers to return.
    /// </summary>
    [SkipDictionaryKeyPolicy]
    [KeyValidationRegex(@"^[a-zA-Z0-9!#$%&'*+-.^_`|~]+$")]
    public Dictionary<string, string> Headers { get; set; } = new();

    /// <summary>
    /// Gets or sets the response body to return.
    /// </summary>
    public string Body { get; set; } = string.Empty;
}
