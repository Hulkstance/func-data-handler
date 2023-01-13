using System.Text;

namespace DataHandler.Results.Errors;

/// <summary>
/// Represents a set of errors produced by an operation.
/// </summary>
/// <param name="Errors">The errors.</param>
/// <param name="Message">The custom error message, if any.</param>
/// <remarks>Used in place of <see cref="AggregateException"/>.</remarks>
public record AggregateError(IReadOnlyCollection<IResult> Errors, string Message = "One or more errors occurred.")
    : ResultError(BuildMessage(Message, Errors))
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateError"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">The errors.</param>
    public AggregateError(string message, params IResult[] errors)
        : this(errors, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateError"/> class.
    /// </summary>
    /// <param name="errors">The errors.</param>
    public AggregateError(params IResult[] errors)
        : this((IReadOnlyCollection<IResult>)errors)
    {
    }

    private static string BuildMessage(string message, IReadOnlyCollection<IResult> errors)
    {
        var sb = new StringBuilder(message);
        sb.AppendLine();

        var index = 0;
        foreach (var error in errors)
        {
            if (error.IsSuccess)
            {
                continue;
            }

            sb.Append($"[{index}]: ");
            var errorLines = (error.Error.ToString() ?? "Unknown").Split('\n');
            foreach (var errorLine in errorLines)
            {
                sb.Append('\t');
                sb.AppendLine(errorLine);
            }

            ++index;
        }

        return sb.ToString();
    }
}
