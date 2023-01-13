namespace DataHandler.Results;

/// <summary>
/// Acts as a base class for result errors.
/// </summary>
public abstract record ResultError(string Message) : IResultError;
