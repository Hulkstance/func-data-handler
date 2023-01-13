namespace DataHandler.Results.Errors;

/// <summary>
/// Represents an error raised when an error occurs during deserialization.
/// </summary>
/// <param name="Message">The error message.</param>
public record DeserializationError(string Message = "An error occurred during deserialization.")
    : ResultError(Message);
