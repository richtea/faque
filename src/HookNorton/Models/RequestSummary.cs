using System.Text.Json.Serialization;

namespace HookNorton.Models;

/// <summary>
/// Summary information about a captured request (used in list view).
/// </summary>
[PublicAPI]
public class RequestSummary
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
    /// Gets or sets a shortened excerpt of the request body.
    /// </summary>
    public string BodyExcerpt { get; set; } = string.Empty;

    /// <summary>
    /// Creates a summary from a full request record.
    /// </summary>
    /// <param name="record">The full request record.</param>
    /// <returns>A new <see cref="RequestSummary"/> instance.</returns>
    public static RequestSummary FromRequestRecord(RequestRecord record)
    {
        const int ExcerptLength = 200;
        var bodyExcerpt = record.Body.Length > ExcerptLength
            ? record.Body[..ExcerptLength]
            : record.Body;

        return new RequestSummary
        {
            Id = record.Id,
            Timestamp = record.Timestamp,
            Method = record.Method,
            Path = record.Path,
            QueryString = record.QueryString,
            BodyExcerpt = bodyExcerpt,
        };
    }
}
