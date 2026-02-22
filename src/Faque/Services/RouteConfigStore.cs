using System.Collections.Concurrent;
using Faque.Common;
using Faque.Models;

namespace Faque.Services;

/// <summary>
/// Thread-safe store for route configurations with optimistic concurrency control.
/// </summary>
public class RouteConfigStore
{
    private readonly ConcurrentDictionary<string, RouteConfiguration> _routes = new();

    private readonly Lock _notificationLock = new();

    /// <summary>
    /// Event raised when routes are modified (for persistence notification).
    /// </summary>
    public event Action RoutesChanged
    {
        add
        {
            lock (_notificationLock)
            {
                RoutesChangedInternal += value;
            }
        }

        remove
        {
            lock (_notificationLock)
            {
                RoutesChangedInternal -= value;
            }
        }
    }

    private event Action? RoutesChangedInternal;

    /// <summary>
    /// Gets all routes.
    /// </summary>
    /// <returns>A list of all routes.</returns>
    public IReadOnlyList<RouteConfiguration> GetAll()
    {
        return _routes.Values.ToList();
    }

    /// <summary>
    /// Gets a specific route by method and path pattern.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="pathPattern">The path pattern.</param>
    /// <returns>A result containing the route configuration if found, otherwise an error.</returns>
    public Result<RouteConfiguration> Get(string method, string pathPattern)
    {
        var key = MakeKey(method, pathPattern);
        if (_routes.TryGetValue(key, out var route))
        {
            return Result<RouteConfiguration>.Success(route);
        }

        return Result<RouteConfiguration>.Failure(Errors.Routes.NotFound(method, pathPattern));
    }

    /// <summary>
    /// Creates or updates a route with optimistic concurrency control.
    /// </summary>
    /// <param name="route">The route configuration to save.</param>
    /// <param name="expectedVersion">The expected version for concurrency control (null for new routes).</param>
    /// <returns>A result containing the saved route configuration, or an error if validation or concurrency check fails.</returns>
    public Result<RouteConfiguration> Upsert(RouteConfiguration route, int? expectedVersion)
    {
        var key = MakeKey(route.Method, route.PathPattern);

        // Validate the route
        var validationResult = ValidateRoute(route);
        if (validationResult.IsFailure)
        {
            return Result<RouteConfiguration>.Failure(validationResult.Error);
        }

        RouteConfiguration savedRoute;

        if (expectedVersion == null)
        {
            // Route is new, or no version is specified - allow creation/update
            savedRoute = _routes.AddOrUpdate(
                key,
                _ =>
                {
                    route.Version = 1;
                    return route;
                },
                (_, existing) =>
                {
                    route.Version = existing.Version + 1;
                    return route;
                });
        }
        else
        {
            // Update with concurrency check
            var updated = false;
            savedRoute = _routes.AddOrUpdate(
                key,
                _ => route,
                (_, existing) =>
                {
                    if (existing.Version == expectedVersion.Value)
                    {
                        route.Version = existing.Version + 1;
                        updated = true;
                        return route;
                    }

                    // Version mismatch - return existing (will be detected below)
                    return existing;
                });

            // If we expected a version but didn't update, it's a conflict
            if (!updated && (savedRoute.Version != route.Version))
            {
                return Result<RouteConfiguration>.Failure(
                    Errors.Routes.ConcurrentModification(route.Method, route.PathPattern));
            }
        }

        NotifyRoutesChanged();
        return Result<RouteConfiguration>.Success(savedRoute);
    }

    /// <summary>
    /// Deletes a specific route.
    /// </summary>
    /// <param name="method">The HTTP method of the route.</param>
    /// <param name="pathPattern">The path pattern of the route.</param>
    /// <returns>A successful <see cref="Result" /> if deleted, otherwise an error if not found.</returns>
    public Result Delete(string method, string pathPattern)
    {
        var key = MakeKey(method, pathPattern);
        if (_routes.TryRemove(key, out _))
        {
            NotifyRoutesChanged();
            return Result.Success();
        }

        return Result.Failure(Errors.Routes.NotFound(method, pathPattern));
    }

    /// <summary>
    /// Clears all route configurations.
    /// </summary>
    public void Clear()
    {
        _routes.Clear();
        NotifyRoutesChanged();
    }

    /// <summary>
    /// Loads routes into the store (replaces existing routes).
    /// </summary>
    /// <param name="routes">The routes to load.</param>
    public void LoadRoutes(IEnumerable<RouteConfiguration> routes)
    {
        _routes.Clear();
        foreach (var route in routes)
        {
            var key = MakeKey(route.Method, route.PathPattern);
            _routes[key] = route;
        }
    }

    private static string MakeKey(string method, string pathPattern)
    {
        return $"{method.ToUpperInvariant()}:{pathPattern}";
    }

    private static Result ValidateRoute(RouteConfiguration route)
    {
        var validMethods = new[] { "GET", "POST", "PUT", "PATCH", "DELETE", "HEAD", "OPTIONS" };
        if (!validMethods.Contains(route.Method.ToUpperInvariant()))
        {
            return Errors.Routes.InvalidMethod(route.Method);
        }

        if (route.Response.StatusCode is < 100 or > 599)
        {
            return Errors.Routes.InvalidStatusCode(route.Response.StatusCode);
        }

        return Result.Success();
    }

    private void NotifyRoutesChanged()
    {
        lock (_notificationLock)
        {
            RoutesChangedInternal?.Invoke();
        }
    }
}
