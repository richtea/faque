using System.Text.RegularExpressions;
using HookNorton.Common;
using HookNorton.Models;

namespace HookNorton.Services;

/// <summary>
/// Matches incoming requests against configured route patterns.
/// Supports * for single-segment wildcards and ** for multi-segment wildcards.
/// </summary>
public class RouteMatcher
{
    /// <summary>
    /// Finds the first enabled route that matches the given method and path.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="path">The request path.</param>
    /// <param name="routes">The collection of configured routes.</param>
    /// <returns>A result containing the matching route or an error if no match is found.</returns>
    public Result<RouteConfiguration> FindMatch(string method, string path, IEnumerable<RouteConfiguration> routes)
    {
        var enabledRoutes = routes.Where(r => r.Enabled && r.Method.Equals(method, StringComparison.OrdinalIgnoreCase));

        foreach (var route in enabledRoutes)
        {
            if (IsMatch(path, route.PathPattern))
            {
                return Result<RouteConfiguration>.Success(route);
            }
        }

        return Result<RouteConfiguration>.Failure(Errors.Routes.NotFound(method, path));
    }

    /// <summary>
    /// Determines if a path matches a pattern.
    /// </summary>
    /// <param name="path">The request path.</param>
    /// <param name="pattern">The pattern with optional * or ** wildcards.</param>
    /// <returns>True if the path matches the pattern.</returns>
    private bool IsMatch(string path, string pattern)
    {
        // Exact match takes precedence
        if (path.Equals(pattern, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // Convert pattern to regex
        var regexPattern = ConvertPatternToRegex(pattern);
        return Regex.IsMatch(path, regexPattern, RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// Converts a glob-style pattern to a regex pattern.
    /// * matches any characters within a single path segment (not including /).
    /// ** matches any characters across multiple path segments (including /).
    /// </summary>
    private string ConvertPatternToRegex(string pattern)
    {
        // Escape special regex characters except * and /
        var escaped = Regex.Escape(pattern);

        // Replace escaped ** with a placeholder to distinguish from *
        escaped = escaped.Replace(@"\*\*", "{{DOUBLESTAR}}");

        // Replace escaped * with regex for single segment (not including /)
        escaped = escaped.Replace(@"\*", @"[^/]*");

        // Replace placeholder with regex for multiple segments (including /)
        escaped = escaped.Replace("{{DOUBLESTAR}}", @".*");

        // Anchor to start and end
        return $"^{escaped}$";
    }
}
