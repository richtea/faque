using System.Text;
using Faque.Services;
using Microsoft.AspNetCore.Mvc;

namespace Faque.Middleware;

/// <summary>
/// Extension methods for adding FakeApiMiddleware to the pipeline.
/// </summary>
public static class FakeApiMiddlewareExtensions
{
    /// <summary>
    /// Adds the <see cref="FakeApiMiddleware" /> to the application pipeline.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The application builder with the middleware added.</returns>
    public static IApplicationBuilder UseFakeApi(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<FakeApiMiddleware>();
    }
}
