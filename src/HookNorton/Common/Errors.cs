namespace HookNorton.Common;

/// <summary>
/// Provides common error definitions for the application.
/// </summary>
public static class Errors
{
    /// <summary>
    /// Errors related to route configuration.
    /// </summary>
    public static class Routes
    {
        /// <summary>
        /// Creates an error for when a route is not found.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="pathPattern">The path pattern.</param>
        /// <returns>A Not Found error.</returns>
        public static Error NotFound(string method, string pathPattern) =>
            Error.NotFound(
                "Route.NotFound",
                $"No route configured for {method} {pathPattern}");

        /// <summary>
        /// Creates an error for an invalid HTTP method.
        /// </summary>
        /// <param name="method">The invalid method.</param>
        /// <returns>A Validation error.</returns>
        public static Error InvalidMethod(string method) =>
            Error.Validation(
                "Route.InvalidMethod",
                $"HTTP method must be one of: GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS. Received: {method}");

        /// <summary>
        /// Creates an error for an invalid status code.
        /// </summary>
        /// <param name="statusCode">The invalid status code.</param>
        /// <returns>A Validation error.</returns>
        public static Error InvalidStatusCode(int statusCode) =>
            Error.Validation(
                "Route.InvalidStatusCode",
                $"Response statusCode must be between 100 and 599. Received: {statusCode}");

        /// <summary>
        /// Creates an error for a missing response configuration.
        /// </summary>
        /// <returns>A Validation error.</returns>
        public static Error MissingResponse() =>
            Error.Validation(
                "Route.MissingResponse",
                "Response configuration is required");

        /// <summary>
        /// Creates an error for concurrent modification of a route.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="pathPattern">The path pattern.</param>
        /// <returns>A Conflict error.</returns>
        public static Error ConcurrentModification(string method, string pathPattern) =>
            Error.Conflict(
                "Route.ConcurrentModification",
                $"The route {method} {pathPattern} was modified by another request");
    }

    /// <summary>
    /// Errors related to request history.
    /// </summary>
    public static class Requests
    {
        /// <summary>
        /// Creates an error for when a request is not found.
        /// </summary>
        /// <param name="id">The request ID.</param>
        /// <returns>A Not Found error.</returns>
        public static Error NotFound(string id) =>
            Error.NotFound(
                "Request.NotFound",
                $"No request found with ID {id}");

        /// <summary>
        /// Creates an error for when request persistence fails.
        /// </summary>
        /// <param name="id">The request ID.</param>
        /// <param name="reason">The failure reason.</param>
        /// <returns>A Failure error.</returns>
        public static Error PersistenceFailed(string id, string reason) =>
            Error.Failure(
                "Request.PersistenceFailed",
                $"Failed to persist request {id}: {reason}");
    }

    /// <summary>
    /// Errors related to general data persistence.
    /// </summary>
    public static class Persistence
    {
        /// <summary>
        /// Creates an error for when loading data fails.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="reason">The failure reason.</param>
        /// <returns>A Failure error.</returns>
        public static Error LoadFailed(string path, string reason) =>
            Error.Failure(
                "Persistence.LoadFailed",
                $"Failed to load data from {path}: {reason}");

        /// <summary>
        /// Creates an error for when saving data fails.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="reason">The failure reason.</param>
        /// <returns>A Failure error.</returns>
        public static Error SaveFailed(string path, string reason) =>
            Error.Failure(
                "Persistence.SaveFailed",
                $"Failed to save data to {path}: {reason}");
    }
}
