using Faque.Common;
using Microsoft.AspNetCore.Mvc;

namespace Faque.Controllers;

/// <summary>
/// Extension methods for converting Result types to ActionResult.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a <see cref="Result{T}" /> to an <see cref="IActionResult" />.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="controller">The controller instance.</param>
    /// <param name="instance">The optional request path instance for problem details.</param>
    /// <returns>The appropriate <see cref="IActionResult" />.</returns>
    public static IActionResult ToActionResult<T>(
        this Result<T> result,
        ControllerBase controller,
        string? instance = null)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result.Value);
        }

        return GetErrorActionResult(result.Error, controller, instance);
    }

    /// <summary>
    /// Converts a <see cref="Result" /> to an <see cref="IActionResult" />.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <param name="controller">The controller instance.</param>
    /// <param name="instance">The optional request path instance for problem details.</param>
    /// <returns>The appropriate <see cref="IActionResult" />.</returns>
    public static IActionResult ToActionResult(this Result result, ControllerBase controller, string? instance = null)
    {
        if (result.IsSuccess)
        {
            return controller.NoContent();
        }

        return GetErrorActionResult(result.Error, controller, instance);
    }

    private static ObjectResult GetErrorActionResult(Error error, ControllerBase controller, string? instance)
    {
        return error.Type switch
        {
            ErrorType.NotFound => controller.Problem(
                error.Message,
                instance,
                StatusCodes.Status404NotFound),
            ErrorType.Validation => controller.Problem(
                error.Message,
                instance,
                StatusCodes.Status422UnprocessableEntity),
            ErrorType.Conflict => controller.Problem(
                error.Message,
                instance,
                StatusCodes.Status409Conflict),
            _ => controller.Problem(
                error.Message,
                instance,
                StatusCodes.Status500InternalServerError),
        };
    }
}
