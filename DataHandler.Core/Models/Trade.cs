namespace DataHandler.Core.Models;

public record TradeRequest(string Operation, string Instrument);

public record TradeResponse(decimal Price, decimal Quantity);
