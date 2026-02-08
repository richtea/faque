namespace HookNorton.Common;

/// <summary>
/// Specifies the type of error.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// The error is not one of the defined types.
    /// </summary>
    Failure,

    /// <summary>
    /// The error is a validation error.
    /// </summary>
    Validation,

    /// <summary>
    /// The resource was not found.
    /// </summary>
    NotFound,

    /// <summary>
    /// The resource already exists, or a state conflict occurred.
    /// </summary>
    Conflict,
}
