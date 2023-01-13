using System.Diagnostics.CodeAnalysis;

namespace DataHandler.Results;

/// <summary>
/// Represents the public API of an interface.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets a value indicating whether the result was successful.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    bool IsSuccess { get; }

    /// <summary>
    /// Gets the error, if any.
    /// </summary>
    IResultError? Error { get; }

    /// <summary>
    /// Gets the inner result, if any.
    /// </summary>
    IResult? Inner { get; }
}
