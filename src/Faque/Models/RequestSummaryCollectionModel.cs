namespace Faque.Models;

/// <summary>
/// Represents a collection of captured requests.
/// </summary>
[PublicAPI]
public class RequestSummaryCollectionModel
{
    /// <summary>
    /// The total number of requests captured.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// The captured requests.
    /// </summary>
    public IEnumerable<RequestSummary> Requests { get; init; } = [];
}
