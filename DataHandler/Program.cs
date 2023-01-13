using DataHandler.Core;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;

const string outputTemplate = "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console(theme: SystemConsoleTheme.Colored, outputTemplate: outputTemplate)
    .CreateLogger();

var client = new ClientFactory(new SerilogLoggerFactory());

await client.SubscribeToTradesAsync(tradeResult =>
{
    Log.Information("TradeResult: {@TradeResult}", tradeResult);
    return ValueTask.CompletedTask;
});

Console.ReadLine();

// TODO: Make it work with binary data too
await client.EmitAsync("""
{
    "op": "trade",
    "instrument": "BTC/USDT"
}
""");
await client.EmitAsync("""
{
    "op": "ticker",
    "instrument": "BTC/USDT"
}
""");
await client.EmitAsync("""
{
    "op": "orderbook",
    "instrument": "BTC/USDT"
}
""");

await client.EmitAsync("""
{
    "op": "trade",
    "instrument": "BTC/USDT"
}
""");

Console.ReadLine();
