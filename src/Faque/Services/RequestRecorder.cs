using System.Collections.Concurrent;
using Faque.Common;
using Faque.Models;
using Faque.Startup;
using Microsoft.Extensions.Options;
using UUIDNext;

namespace Faque.Services;

/// <summary>
/// Records incoming requests with bounded history using UUIDv7 for chronological ordering.
/// </summary>
public class RequestRecorder
{
    private readonly FaqueOptions _options;
    private readonly ConcurrentDictionary<string, RequestRecord> _requests = new();
    private readonly ConcurrentQueue<string> _requestOrder = new();
    private readonly object _lock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestRecorder"/> class.
    /// </summary>
    /// <param name="options">The application options.</param>
    public RequestRecorder(IOptions<FaqueOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// Records a new request.
    /// </summary>
    /// <param name="method">The HTTP method of the request.</param>
    /// <param name="path">The request path.</param>
    /// <param name="queryString">The query string parameters.</param>
    /// <param name="headers">The request headers.</param>
    /// <param name="body">The request body.</param>
    /// <returns>A result containing the recorded request record.</returns>
    public Result<RequestRecord> RecordRequest(
        string method,
        string path,
        string queryString,
        Dictionary<string, string> headers,
        string body)
    {
        // Truncate body if needed
        var truncatedBody = body.Length > _options.MaxBodySize
            ? body[.._options.MaxBodySize]
            : body;

        var record = new RequestRecord
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql).ToString(),
            Timestamp = DateTimeOffset.UtcNow,
            Method = method,
            Path = path,
            QueryString = queryString,
            Headers = headers,
            Body = truncatedBody,
        };

        lock (_lock)
        {
            _requests[record.Id] = record;
            _requestOrder.Enqueue(record.Id);

            // Enforce bounded history
            while (_requestOrder.Count > _options.MaxRequestHistory)
            {
                if (_requestOrder.TryDequeue(out var oldestId))
                {
                    _requests.TryRemove(oldestId, out _);
                }
            }
        }

        return Result<RequestRecord>.Success(record);
    }

    /// <summary>
    /// Gets a specific request by ID.
    /// </summary>
    /// <param name="id">The request ID.</param>
    /// <returns>A result containing the request record or an error if not found.</returns>
    public Result<RequestRecord> GetRequest(string id)
    {
        if (_requests.TryGetValue(id, out var record))
        {
            return Result<RequestRecord>.Success(record);
        }

        return Result<RequestRecord>.Failure(Errors.Requests.NotFound(id));
    }

    /// <summary>
    /// Gets all requests ordered by timestamp descending (newest first).
    /// Returns at most N requests per configuration.
    /// </summary>
    /// <returns>A list of request records.</returns>
    public IReadOnlyList<RequestRecord> GetAllRequests()
    {
        return _requests.Values
            .OrderByDescending(r => r.Id) // UUIDv7 is chronologically sortable
            .Take(_options.MaxRequestHistory)
            .ToList();
    }

    /// <summary>
    /// Gets request summaries ordered by timestamp descending.
    /// </summary>
    /// <returns>A list of request summaries.</returns>
    public IReadOnlyList<RequestSummary> GetRequestSummaries()
    {
        return GetAllRequests()
            .Select(RequestSummary.FromRequestRecord)
            .ToList();
    }

    /// <summary>
    /// Clears all recorded requests.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _requests.Clear();
            while (_requestOrder.TryDequeue(out _))
            {
                // Clear the queue
            }
        }
    }

    /// <summary>
    /// Loads requests from a collection (used during startup).
    /// Maintains chronological order and enforces bounded history.
    /// </summary>
    /// <param name="requests">The collection of requests to load.</param>
    public void LoadRequests(IEnumerable<RequestRecord> requests)
    {
        lock (_lock)
        {
            Clear();

            // Sort by UUIDv7 (chronological) and take latest N
            var sortedRequests = requests
                .OrderBy(r => r.Id)
                .TakeLast(_options.MaxRequestHistory)
                .ToList();

            foreach (var request in sortedRequests)
            {
                _requests[request.Id] = request;
                _requestOrder.Enqueue(request.Id);
            }
        }
    }

    /// <summary>
    /// Gets the IDs of all requests in chronological order (oldest first).
    /// </summary>
    /// <returns>A list of request IDs.</returns>
    public IReadOnlyList<string> GetAllRequestIds()
    {
        lock (_lock)
        {
            return _requestOrder.ToList();
        }
    }
}
