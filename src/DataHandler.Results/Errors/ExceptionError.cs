namespace DataHandler.Results.Errors;

/// <summary>
///     Represents an error caused by an exception.
/// </summary>
public sealed record ExceptionError : ResultError
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ExceptionError" /> class.
    /// </summary>
    /// <param name="exception">The exception that caused the error.</param>
    public ExceptionError(Exception exception)
        : base(exception.Message)
    {
        Exception = exception;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ExceptionError" /> class.
    /// </summary>
    /// <param name="exception">The exception that caused the error.</param>
    /// <param name="message">The custom human-readable error message.</param>
    public ExceptionError(Exception exception, string message)
        : base(message)
    {
        Exception = exception;
    }

    /// <summary>
    ///     Gets the exception that caused the error.
    /// </summary>
    public Exception Exception { get; }
}

