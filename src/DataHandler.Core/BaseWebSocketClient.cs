using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using DataHandler.Core.Models;
using DataHandler.Core.Models.Responses;
using Microsoft.Extensions.Logging;
using DataReceivedEventArgs = DataHandler.Core.Events.DataReceivedEventArgs;

namespace DataHandler.Core;

public abstract class BaseWebSocketClient : IDisposable
{
    private readonly ILogger<BaseWebSocketClient> _logger;
    private readonly SubscriptionCollection _subscriptions = new();
    private readonly WebSocketClient _webSocketClient = new();

    private long _lastRequestId;
    private bool _disposed;

    protected BaseWebSocketClient(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<BaseWebSocketClient>();

        _webSocketClient.DataReceivedAsync += OnDataReceived;
    }

    #region Dispose pattern

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Checks if this object has been disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the object has been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void DoDisposeChecks()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(BaseWebSocketClient));
        }
    }

    /// <summary>
    ///     Disposes of managed and unmanaged resources.
    /// </summary>
    /// <param name="disposing">A value indicating whether or not to dispose of managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _webSocketClient.DataReceivedAsync -= OnDataReceived;
        }

        _disposed = true;
    }

    #endregion

    public async ValueTask EmitAsync(string json)
    {
        await _webSocketClient.EmitAsync(json);
    }

    protected ValueTask<Subscription> SubscribeAsync<T>(object request, Func<DataEvent<T>, ValueTask> dataHandler)
    {
        var subscription = new Subscription(Guid.NewGuid(), request, async messageEvent =>
        {
            var result = messageEvent.Data.Deserialize<SocketResponse<T>>(new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            });

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to deserialize: {Message}", result.Error!.Message);
                return;
            }

            if (result.IsDefined(out var data))
            {
                await dataHandler(new DataEvent<T>(data.Data, messageEvent.ReceiveTimestamp));
            }
        });

        _subscriptions.Add(subscription);

        return ValueTask.FromResult(subscription);
    }

    protected abstract bool MessageMatchesHandler(string data, object request);

    protected long GetNextRequestId()
    {
        return Interlocked.Increment(ref _lastRequestId);
    }

    private async ValueTask OnDataReceived(DataReceivedEventArgs e)
    {
        var timestamp = DateTimeOffset.Now;

        var messageEvent = new MessageEvent(e.Message, timestamp);
        foreach (var subscription in _subscriptions
                     .GetAll()
                     .Where(subscription => MessageMatchesHandler(messageEvent.Data, subscription.Request)))
        {
            var userProcessTime = await GetProcessTime(async () => await subscription.DataHandler(messageEvent));

            if (userProcessTime.TotalMilliseconds > 500)
            {
                _logger.LogTrace("Message processing slow ({UserProcessTimeMs} ms user code), consider " +
                                 "offloading data handling to another thread. Data from this socket may arrive late " +
                                 "or not at all if message processing is continuously slow.",
                    userProcessTime.TotalMilliseconds);
            }
        }
    }

    private async Task<TimeSpan> GetProcessTime(Func<ValueTask> func)
    {
        var userProcessTime = Stopwatch.StartNew();

        await func();

        userProcessTime.Stop();
        return userProcessTime.Elapsed;
    }
}
