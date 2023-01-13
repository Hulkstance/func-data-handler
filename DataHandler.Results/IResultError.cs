namespace DataHandler.Results;

/// <summary>
/// Represents an error returned by a result.
/// </summary>
public interface IResultError
{
    /// <summary>
    /// Gets the human-readable error message.
    /// </summary>
    string Message { get; }
}
