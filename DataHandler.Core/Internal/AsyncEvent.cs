using Microsoft.Extensions.Logging;

namespace DataHandler.Core.Internal;

public sealed class AsyncEvent<TEventArgs> where TEventArgs : EventArgs
{
    private readonly List<AsyncEventInvocator<TEventArgs>> _handlers = new();

    public bool HasHandlers => _handlers.Count > 0;

    public void AddHandler(Func<TEventArgs, ValueTask> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        _handlers.Add(new AsyncEventInvocator<TEventArgs>(default, handler));
    }

    public void AddHandler(Action<TEventArgs> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        _handlers.Add(new AsyncEventInvocator<TEventArgs>(handler, default));
    }

    public async ValueTask InvokeAsync(TEventArgs eventArgs)
    {
        var tasks = _handlers
            .Select(handler => handler.InvokeAsync(eventArgs))
            .Select(task => task.AsTask()).ToList();

        await Task.WhenAll(tasks);
    }

    public void RemoveHandler(Func<TEventArgs, ValueTask> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        _handlers.RemoveAll(h => h.WrapsHandler(handler));
    }

    public void RemoveHandler(Action<TEventArgs> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        _handlers.RemoveAll(h => h.WrapsHandler(handler));
    }

    public async ValueTask TryInvokeAsync(TEventArgs eventArgs, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(eventArgs);
        ArgumentNullException.ThrowIfNull(logger);

        try
        {
            await InvokeAsync(eventArgs).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Error while invoking event ({EventType})", typeof(TEventArgs));
        }
    }
}
