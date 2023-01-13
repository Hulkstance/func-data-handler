namespace DataHandler.Results.Errors;

/// <summary>
/// Represents an error arising from an argument to a function.
/// </summary>
/// <param name="Name">The name of the argument.</param>
/// <param name="Message">The error message.</param>
/// <remarks>Used in place of <see cref="ArgumentException"/>.</remarks>
public abstract record ArgumentError(string Name, string Message)
    : ResultError($"Error in argument {Name}: {Message}");
