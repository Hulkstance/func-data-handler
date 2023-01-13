using DataHandler.Dummy;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;

const string outputTemplate = "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console(theme: SystemConsoleTheme.Colored, outputTemplate: outputTemplate)
    .CreateLogger();

var client = new DummyWebSocketClient(new SerilogLoggerFactory());

await client.SubscribeToTradesAsync(tradeResult =>
{
    Log.Information("#1: {@TradeResult}", tradeResult);
    return ValueTask.CompletedTask;
});

await client.SubscribeToTradesAsync(async tradeResult =>
{
    await Task.Delay(1200);

    Log.Information("#2: {@TradeResult}", tradeResult);
});

// TODO: Make it work with binary data too
await client.EmitAsync("""
{
    "id": "1",
    "method": "subscription",
    "channel": "trade",
    "data": {
        "price": "5.8",
        "quantity": "1.2"
    }
}
""");
await client.EmitAsync("""
{
    "id": "2",
    "method": "subscription",
    "channel": "trade",
    "data": {
        "price": "1.2",
        "quantity": "0.5"
    }
}
""");
await client.EmitAsync("""
{
    "id": "3",
    "method": "subscription",
    "channel": "orderbook",
    "data": "two"
}
""");
await client.EmitAsync("""
{
    "id": "1",
    "method": "subscription",
    "channel": "trade",
    "data": {
        "price": "100.1",
        "quantity": "5.5"
    }
}
""");

Console.ReadLine();
