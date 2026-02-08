using System.Text.Json.Serialization;

namespace HookNorton.Models;

/// <summary>
/// Represents the response configuration for a route.
/// </summary>
public class RouteResponse
{
    /// <summary>
    /// Gets or sets the HTTP status code to return.
    /// </summary>
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; } = 200;

    /// <summary>
    /// Gets or sets the HTTP headers to return.
    /// </summary>
    [JsonPropertyName("headers")]
    [UsedImplicitly]
    public Dictionary<string, string> Headers { get; set; } = new();

    /// <summary>
    /// Gets or sets the response body to return.
    /// </summary>
    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;
}
