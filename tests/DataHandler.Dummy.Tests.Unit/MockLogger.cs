using Microsoft.Extensions.Logging;

namespace DataHandler.Dummy.Tests.Unit;

// https://github.com/nsubstitute/NSubstitute/issues/597
public abstract class MockLogger<T> : ILogger<T>
{
    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        => Log(logLevel, formatter(state, exception), exception);

    public abstract void Log(LogLevel logLevel, object state, Exception? exception = null);

    public virtual bool IsEnabled(LogLevel logLevel) => true;

    public abstract IDisposable? BeginScope<TState>(TState state) where TState : notnull;
}
