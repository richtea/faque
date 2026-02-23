namespace Faque.Common;

/// <summary>
/// Represents an error that occurred during an operation.
/// </summary>
/// <param name="Code">The unique error code.</param>
/// <param name="Message">A human-readable error message.</param>
/// <param name="Type">The category of the error.</param>
public record Error(string Code, string Message, ErrorType Type)
{
    /// <summary>
    /// Creates a Conflict error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="Error" /> of type <see cref="ErrorType.Conflict" />.</returns>
    public static Error Conflict(string code, string message)
    {
        return new Error(code, message, ErrorType.Conflict);
    }

    /// <summary>
    /// Creates a Failure error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="Error" /> of type <see cref="ErrorType.Failure" />.</returns>
    public static Error Failure(string code, string message)
    {
        return new Error(code, message, ErrorType.Failure);
    }

    /// <summary>
    /// Creates a Not Found error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="Error" /> of type <see cref="ErrorType.NotFound" />.</returns>
    public static Error NotFound(string code, string message)
    {
        return new Error(code, message, ErrorType.NotFound);
    }

    /// <summary>
    /// Creates a Validation error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="Error" /> of type <see cref="ErrorType.Validation" />.</returns>
    public static Error Validation(string code, string message)
    {
        return new Error(code, message, ErrorType.Validation);
    }
}
