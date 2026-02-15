namespace Faque.Models;

/// <summary>
/// Complete information about a captured request.
/// </summary>
[PublicAPI]
public class RequestRecord
{
    /// <summary>
    /// Gets or sets the unique identifier for the request.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the request was captured.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the HTTP method used for the request.
    /// </summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the request path.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the query string part of the request.
    /// </summary>
    public string QueryString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of request headers.
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new();

    /// <summary>
    /// Gets or sets the request body.
    /// </summary>
    public string Body { get; set; } = string.Empty;
}
