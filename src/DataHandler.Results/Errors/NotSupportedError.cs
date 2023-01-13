namespace DataHandler.Results.Errors;

/// <summary>
/// Represents an error raised when an attempt to perform an unsupported action is made.
/// </summary>
/// <param name="Message">The error message.</param>
/// <remarks>Used in place of <see cref="NotSupportedException"/>.</remarks>
public record NotSupportedError(string Message = "The requested action is not supported.")
    : ResultError(Message);
