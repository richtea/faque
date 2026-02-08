using System.Text.Json.Serialization;

namespace HookNorton.Models;

/// <summary>
/// Complete information about a captured request.
/// </summary>
public class RequestRecord
{
    /// <summary>
    /// Gets or sets the unique identifier for the request.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the request was captured.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the HTTP method used for the request.
    /// </summary>
    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the request path.
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the query string part of the request.
    /// </summary>
    [JsonPropertyName("queryString")]
    public string QueryString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of request headers.
    /// </summary>
    [JsonPropertyName("headers")]
    public Dictionary<string, string> Headers { get; set; } = new();

    /// <summary>
    /// Gets or sets the request body.
    /// </summary>
    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;
}
