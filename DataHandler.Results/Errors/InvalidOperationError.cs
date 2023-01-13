namespace DataHandler.Results.Errors;

/// <summary>
/// Represents an error raised when an attempt to perform an invalid operation is made.
/// </summary>
/// <param name="Message">The error message.</param>
/// <remarks>Used in place of <see cref="InvalidOperationException"/>.</remarks>
public record InvalidOperationError(string Message = "The requested operation is invalid.")
    : ResultError(Message);
