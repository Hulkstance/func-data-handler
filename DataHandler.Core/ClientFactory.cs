using System.Text.Json.Nodes;
using DataHandler.Core.Events;
using DataHandler.Core.Models;
using Microsoft.Extensions.Logging;

namespace DataHandler.Core;

public sealed class ClientFactory : IDisposable
{
    private readonly ILogger<ClientFactory> _logger;
    private readonly SubscriptionCollection _subscriptions;
    private readonly SocketFactory _socketFactory;

    public ClientFactory(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ClientFactory>();

        _subscriptions = new SubscriptionCollection();

        _socketFactory = new SocketFactory();
        _socketFactory.DataReceivedAsync += OnDataReceived;
    }

    public void Dispose()
    {
        _socketFactory.DataReceivedAsync -= OnDataReceived;
    }

    public async ValueTask EmitAsync(string json)
    {
        await _socketFactory.EmitAsync(json);
    }

    public ValueTask SubscribeAsync<T>(object request, Func<DataEvent<T>, ValueTask> dataHandler)
    {
        var subscription = new Subscription(GetNextRequestId(), request, async messageEvent =>
        {
            var result = messageEvent.Data.DeserializeInternal<T>();

            if (result.IsDefined(out var data))
            {
                await dataHandler(new DataEvent<T>(data, messageEvent.ReceiveTimestamp));
            }
        });

        _subscriptions.Add(subscription);
        return ValueTask.CompletedTask;
    }

    public async ValueTask SubscribeToTradesAsync(Func<DataEvent<TradeResponse>, ValueTask> tradeFunc)
    {
        var request = new TradeRequest("trade", "BTC/USDT");
        await SubscribeAsync(request, tradeFunc);
    }

    private bool MessageMatchesHandler(string data, object request)
    {
        var req = (TradeRequest)request;

        var message = JsonNode.Parse(data);
        var op = message?["op"]?.ToString();
        var instrument = message?["instrument"]?.ToString();

        return (op == req.Operation && instrument == req.Instrument);
    }

    private async ValueTask OnDataReceived(DataReceivedEventArgs e)
    {
        var timestamp = DateTimeOffset.Now;

        var messageEvent = new MessageEvent(e.Message, timestamp);
        foreach (var subscription in _subscriptions
                     .GetAll()
                     .Where(subscription => MessageMatchesHandler(messageEvent.Data, subscription.Request)))
        {
            await subscription.DataHandler(messageEvent);
        }
    }

    #region Request IDs

    private uint _lastRequestId;

    public uint GetNextRequestId()
    {
        return Interlocked.Increment(ref _lastRequestId);
    }

    #endregion
}
