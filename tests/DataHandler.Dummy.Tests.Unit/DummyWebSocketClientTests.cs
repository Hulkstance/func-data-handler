using DataHandler.Core;
using DataHandler.Dummy.Models.Responses;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace DataHandler.Dummy.Tests.Unit;

public class DummyWebSocketClientTests
{
    private readonly ITestOutputHelper _output;

    private readonly ILoggerFactory _loggerFactory;
    private readonly MockLogger<BaseWebSocketClient> _logger;
    private readonly DummyWebSocketClient _sut;

    public DummyWebSocketClientTests(ITestOutputHelper output)
    {
        _output = output;

        _loggerFactory = Substitute.For<ILoggerFactory>();
        _logger = Substitute.For<MockLogger<BaseWebSocketClient>>();
        _loggerFactory.CreateLogger<BaseWebSocketClient>().ReturnsForAnyArgs(_logger);

        _sut = new DummyWebSocketClient(_loggerFactory);
    }

    [Fact]
    public async Task SubscribeToTradesAsync_ShouldLogSlowMessageProcessing_WhenGivenDelay()
    {
        // Arrange


        // Act
        await _sut.SubscribeToTradesAsync(async _ =>
        {
            await Task.Delay(1000);
        });

        await _sut.EmitAsync("""
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

        // Assert
        _logger.Received(1).Log(LogLevel.Trace, Arg.Is<string>(s => s.Contains("Message processing slow")));
    }

    [Fact]
    public async Task SubscribeToTradesAsync_ShouldProduceElementsInCorrectOrder_WhenGivenJsonObjects()
    {
        // Arrange
        var responses = new List<(int, TradeResponse)>();

        // Act
        await _sut.SubscribeToTradesAsync(tradeResult =>
        {
            _output.WriteLine($"#1 | Price: {tradeResult.Data.Price} | Quantity: {tradeResult.Data.Quantity}");

            responses.Add((1, new TradeResponse(tradeResult.Data.Price, tradeResult.Data.Quantity)));

            return ValueTask.CompletedTask;
        });

        await _sut.SubscribeToTradesAsync(tradeResult =>
        {
            _output.WriteLine($"#2 | Price: {tradeResult.Data.Price} | Quantity: {tradeResult.Data.Quantity}");

            responses.Add((2, new TradeResponse(tradeResult.Data.Price, tradeResult.Data.Quantity)));

            return ValueTask.CompletedTask;
        });

        await _sut.EmitAsync("""
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
        await _sut.EmitAsync("""
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
        await _sut.EmitAsync("""
        {
            "id": "3",
            "method": "subscription",
            "channel": "orderbook",
            "data": "two"
        }
        """);
        await _sut.EmitAsync("""
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

        // Assert
        responses.Should().HaveCount(3)
            .And.BeEquivalentTo(new List<(int, TradeResponse)>
            {
                (1, new TradeResponse(5.8m, 1.2m)),
                (2, new TradeResponse(1.2m, 0.5m)),
                (1, new TradeResponse(100.1m, 5.5m))
            }, opt => opt.WithStrictOrdering());
    }
}
