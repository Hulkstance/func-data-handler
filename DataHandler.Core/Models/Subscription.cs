namespace DataHandler.Core.Models;

public record Subscription(long Id, object Request, Func<MessageEvent, ValueTask> DataHandler);
