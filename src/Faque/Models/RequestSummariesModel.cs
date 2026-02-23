namespace Faque.Models;

/// <summary>
/// Represents a collection of captured requests.
/// </summary>
[PublicAPI]
public class RequestSummariesModel
{
    /// <summary>
    /// Gets the total number of requests captured.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Gets the captured requests.
    /// </summary>
    public IEnumerable<RequestSummary> Requests { get; init; } = [];
}
