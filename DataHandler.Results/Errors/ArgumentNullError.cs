namespace DataHandler.Results.Errors;

/// <summary>
/// Represents an error arising from an argument being null.
/// </summary>
/// <param name="Name">The name of the argument.</param>
/// <param name="Message">The error message.</param>
/// <remarks>Used in place of <see cref="ArgumentNullException"/>.</remarks>
public record ArgumentNullError(string Name, string Message = "Value may not be null")
    : ArgumentError(Name, Message);
