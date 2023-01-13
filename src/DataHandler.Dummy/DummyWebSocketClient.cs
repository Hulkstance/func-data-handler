using System.Text.Json.Nodes;
using DataHandler.Core;
using DataHandler.Core.Models;
using DataHandler.Core.Models.Requests;
using DataHandler.Dummy.Models.Responses;
using Microsoft.Extensions.Logging;

namespace DataHandler.Dummy;

public sealed class DummyWebSocketClient : BaseWebSocketClient
{
    public DummyWebSocketClient(ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
    }

    public ValueTask<Subscription> SubscribeToTradesAsync(Func<DataEvent<TradeResponse>, ValueTask> tradeFunc)
    {
        var request = new SocketRequest(GetNextRequestId(), "trade");
        return SubscribeAsync(request, tradeFunc);
    }

    protected override bool MessageMatchesHandler(string data, object request)
    {
        var socketRequest = (SocketRequest)request;

        var message = JsonNode.Parse(data);

        var id = message?["id"]?.ToString();
        var method = message?["method"]?.ToString();
        var channel = message?["channel"]?.ToString();

        if (method != "subscription")
        {
            return false;
        }

        return (id == socketRequest.Id.ToString() && channel == socketRequest.Channel);
    }
}
