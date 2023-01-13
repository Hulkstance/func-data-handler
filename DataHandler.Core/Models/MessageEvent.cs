namespace DataHandler.Core.Models;

public record MessageEvent(string Data, DateTimeOffset ReceiveTimestamp);
