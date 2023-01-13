namespace DataHandler.Results.Errors;

/// <summary>
/// Represents an error arising from an argument being invalid.
/// </summary>
/// <param name="Name">The name of the argument.</param>
/// <param name="Message">The error message.</param>
/// <remarks>Used in place of <see cref="ArgumentException"/>.</remarks>
public record ArgumentInvalidError(string Name, string Message) : ArgumentError(Name, Message);
