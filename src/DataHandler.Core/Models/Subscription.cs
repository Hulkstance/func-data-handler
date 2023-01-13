namespace DataHandler.Core.Models;

public record Subscription(Guid Id, object Request, Func<MessageEvent, ValueTask> DataHandler);
